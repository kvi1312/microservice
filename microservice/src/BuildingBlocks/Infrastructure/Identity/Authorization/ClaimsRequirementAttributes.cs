using Microsoft.AspNetCore.Mvc;
using Shared.Common.Constants;

namespace Infrastructure.Identity.Authorization;

public class ClaimsRequirementAttributes : TypeFilterAttribute
{
    public ClaimsRequirementAttributes(FunctionCode function, CommandCode command) : base(typeof(ClaimsRequirementFilter))
    {
        Arguments = new object[] { function, command };
    }
}
