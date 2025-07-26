using Microservice.IDP.Infrastructure.Common;
using Microservices.IDP.Infrastructure.ViewModels;

namespace IdentityServer.Common;

public class PermissionHelper
{
    public static string GetPermission(string functionCode, string commandCode) => string.Join(".", functionCode, commandCode);
    
    public static List<PermissionAddModel> GetAllPermissions()
    {
        var permissions = new List<PermissionAddModel>();
        var functions = SystemConstants.Functions.GetAllFunctions();
        var commands = SystemConstants.Permissions.GetAllCommands();

        foreach (var function in functions)
        {
            foreach (var command in commands)
            {
                permissions.Add(new PermissionAddModel
                {
                    Function = function,
                    Command = command
                });
            }
        }

        return permissions;
    }
}
