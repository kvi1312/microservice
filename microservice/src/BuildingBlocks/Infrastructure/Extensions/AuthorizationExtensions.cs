using Microsoft.AspNetCore.Builder;
using Shared.Common.Constants;

namespace Infrastructure.Extensions;

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequireClaimsAuth(this RouteHandlerBuilder builder,
                                                        FunctionCode function,
                                                        CommandCode command)
    {
        var policy = $"{function}.{command}";
        return builder.RequireAuthorization(policy);
    }
}
