using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamTrack.Core.Data;
using TeamTrack.Core.Entities;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using TeamTrack.Core.Infrastructure;
using Microsoft.AspNetCore.Identity;
using TeamTrack.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using TeamTrack.Api.Tools;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamTrack.Api.Infrastructure;

namespace TeamTrack.Api
{
    public class Startup
    {
        #region Constructor
        public IConfigurationRoot Configuration { get; }
        public IMapper Mapper { get; set; }
        private MapperConfiguration MapperConfiguration { get; set; }

        private SymmetricSecurityKey securityKey;

        #endregion

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<TeamTrackDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, Role>()
                    .AddEntityFrameworkStores<TeamTrackDbContext, int>()
                    .AddDefaultTokenProviders();

            services.AddMvc(opt =>
            {
                opt.UseCentralRoutePrefix(new RouteAttribute("api/v{version}"));
            });

            services.AddTransient<IRepository, Repository>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password = new PasswordOptions
                {
                    RequireDigit = false,
                    RequireUppercase = false,
                    RequireLowercase = false,
                    RequiredLength = 6,
                    RequireNonAlphanumeric = false
                };
            });

            securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Authentication:SecurityKey"]));

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
            });

            MapperConfiguration MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfileConfiguration());
            });

            services.AddSingleton(sp => MapperConfiguration.CreateMapper());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            //env.ConfigureNLog("nlog.config");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<TeamTrackDbContext>().Database.Migrate();
                    var userManager = app.ApplicationServices.GetService<UserManager<User>>();
                    var roleManager = app.ApplicationServices.GetService<RoleManager<Role>>();

                    serviceScope.ServiceProvider.GetService<TeamTrackDbContext>().EnsureSeedData(userManager, roleManager);
                }
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(
                      async context =>
                      {
                          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                          var error = context.Features.Get<IExceptionHandlerFeature>();
                          if (error != null)
                          {
                              await context.Response.WriteAsync($"{Resources.UnexpectedError}. Error: {error.Error.Message}").ConfigureAwait(false);
                          }
                      });
                });
            }

            var tokenProviderOptions = new TokenProviderOptions()
            {
                Path = "/api/token",
                Audience = "teamtrack",
                Issuer = "teamtrack",
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
                Expiration = TimeSpan.FromMinutes(int.Parse(Configuration["Authentication:AccessTokenExpireTimeSpan"]))
            };

            app.UseTokenProvider(tokenProviderOptions);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = tokenProviderOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = tokenProviderOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseIdentity();

            app.UseMvc();
        }
    }
}
