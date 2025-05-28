using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Shared.Configurations;
using System.Text;

namespace OcelotApiGw.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
            services.AddSingleton(jwtSettings);
            return services;
        }

        public static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOcelot(configuration)
                .AddPolly();
        }

        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = configuration["AllowedOrigins"];
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }

        internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var settings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
            if (settings == null || string.IsNullOrEmpty(settings.Key))
            {
                throw new ArgumentNullException($"{nameof(settings)} is not configured");
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = signingKey,
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                RequireExpirationTime = false,
                ClockSkew = TimeSpan.Zero,
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = true;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            return services;
        }
    }
}
