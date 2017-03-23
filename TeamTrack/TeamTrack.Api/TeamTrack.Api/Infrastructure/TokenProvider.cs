using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using TeamTrack.Api.Tools;
using TeamTrack.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using System.Collections.Generic;

namespace TeamTrack.Api.Infrastructure
{
    public class TokenProvider
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly TokenProviderOptions _options;
        private Logger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly UserManager<User> _userManager;

        public TokenProvider(RequestDelegate requestDelegate, IOptions<TokenProviderOptions> options, UserManager<User> userManager)
        {
            _requestDelegate = requestDelegate;
            _userManager = userManager;
            _options = options.Value;

            ThrowIfInvalidOptions(_options);

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            if (_logger == null)
            {
                _logger = LogManager.GetCurrentClassLogger();
            }
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _requestDelegate(context);
            }

            if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync(Resources.BadRequest);
            }

            _logger.Info("Handling request: " + context.Request.Path);

            return GenerateToken(context);
        }

        private async Task GenerateToken(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            try
            {
                var userName = context.Request.Form["userName"];
                var password = context.Request.Form["password"];
                User user = null;

                user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(userName);
                }

                if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(Resources.InvalidUsernamePassword);
                    return;
                }

                var now = DateTime.UtcNow;
                var claims = new List<Claim>(new Claim[] {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, await _options.NonceGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64)
                });

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));
                foreach (var claim in user.Claims) claims.Add(new Claim(claim.ClaimType, claim.ClaimValue));

                var jwt = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(_options.Expiration),
                    signingCredentials: _options.SigningCredentials);

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)_options.Expiration.TotalSeconds
                };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response, _serializerSettings));
            }
            catch (Exception exception)
            {
                _logger.Error("{0} Path: {1}", exception, context.Request.Path);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(Resources.UnexpectedError, _serializerSettings));
            }
        }

        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Path));
            }

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenProviderOptions.Expiration));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.NonceGenerator));
            }
        }

        public static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}
