using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configurations;

namespace Infrastructure.Identity;

public static class ConfigureAuthenAuthorHandler
{

    public static void ConfigureAuthenticationHandler(this IServiceCollection services)
    {
        var configurations = services.GetOptions<ApiConfiguration>("ApiConfiguration");
        if (configurations == null || string.IsNullOrEmpty(configurations.IssuerUri) || string.IsNullOrEmpty(configurations.ApiName))
        {
            throw new ArgumentNullException($"Api configuration is not configured");
        }

        var issuerUri = configurations.IssuerUri;
        var apiName = configurations.ApiName;

        
        services.AddAuthentication("Bearer").AddJwtBearer(opt =>
        {
            opt.Authority = issuerUri;
            opt.RequireHttpsMetadata = false;
            opt.Audience = apiName;
            opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                // JWT specific settings
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateTokenReplay = false
            };
        });
    }

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Bearer", policy =>
            {
                policy.AddAuthenticationSchemes("Bearer");
                policy.RequireAuthenticatedUser();
            });
        });
    }
}
