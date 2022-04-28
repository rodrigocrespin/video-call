using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace VideoCall.Api.Config
{
    public static class ServiceCollectionExtensions
    {
        public static void SetupCors(this IServiceCollection services, IConfiguration configuration, ILogger? logger = null)
        {
            var allowedOrigins = (configuration["AllowedOrigins"] ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries);
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    if (allowedOrigins.Length == 0 || allowedOrigins.Contains("*"))
                    {
                        policy.SetIsOriginAllowed(_ => true);
                        logger?.LogInformation("CORS enabled for all origins");
                    }
                    else
                    {
                        policy.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins(allowedOrigins);
                        logger?.LogInformation("CORS enabled for {@allowedOrigins}", string.Join(", ", allowedOrigins));
                    }

                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("*")
                        .AllowCredentials();
                });
            });
        }
    }
}
