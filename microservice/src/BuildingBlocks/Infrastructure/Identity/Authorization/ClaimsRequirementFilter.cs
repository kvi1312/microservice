using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common.Constants;
using System.Text.Json;

namespace Infrastructure.Identity.Authorization;

public class ClaimsRequirementFilter : IAuthorizationFilter
{
    private readonly CommandCode _commandCode;
    private readonly FunctionCode _functionCode;

    public ClaimsRequirementFilter(FunctionCode functionCode, CommandCode commandCode)
    {
        _functionCode = functionCode;
        _commandCode = commandCode;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var permissionClaims = context.HttpContext.User.Claims.SingleOrDefault(c => c.Type.Equals(SystemConstants.Claims.Permissions));

        if (permissionClaims != null)
        {
            var permissions = JsonSerializer.Deserialize<List<string>>(permissionClaims.Value);

            if (permissions != null && !permissions.Contains(PermissionHelper.GetPermission(_functionCode, _commandCode)))
            {
                context.Result = new ForbidResult();
            }
        }
        else
        {
            context.Result = new ForbidResult();
        }
    }
}
