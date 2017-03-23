using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using TeamTrack.Api.Infrastructure;

namespace TeamTrack.Api.Extensions
{
    public static class TokenProviderAppBuilderExtensions
    {
        public static IApplicationBuilder UseTokenProvider(this IApplicationBuilder app, TokenProviderOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            return app.UseMiddleware<TokenProvider>(Options.Create(options));
        }
    }
}
