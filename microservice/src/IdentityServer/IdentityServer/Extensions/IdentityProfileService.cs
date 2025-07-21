using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityServer.Common;
using Microservice.IDP.Infrastructure.Common;
using Microservice.IDP.Infrastructure.Entities;
using Microservice.IDP.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityServer.Extensions;

public class IdentityProfileService : IProfileService
{
    private readonly IUserClaimsPrincipalFactory<User> _claimFactory;
    private readonly UserManager<User> _userManager;
    private readonly IRepositoryManager _repositoryManager;

    public IdentityProfileService(UserManager<User> userManager, IUserClaimsPrincipalFactory<User> claimFactory, IRepositoryManager repositoryManager)
    {
        _userManager = userManager;
        _claimFactory = claimFactory;
        _repositoryManager = repositoryManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(sub);
        if (user == null)
        {
            throw new ArgumentNullException("User Id not found");
        }

        var principal = await _claimFactory.CreateAsync(user);
        var claims = principal.Claims.ToList();
        var roles = await _userManager.GetRolesAsync(user);

        var permissionQuery = await _repositoryManager.PermissionRepository.GetPermissionByUser(user);
        var permissions = permissionQuery.Select(x => PermissionHelper.GetPermission(x.Function, x.Command));

        claims.Add(new Claim(SystemConstants.Claims.FirstName, user.FirstName));
        claims.Add(new Claim(SystemConstants.Claims.LastName, user.LastName));
        claims.Add(new Claim(SystemConstants.Claims.UserName, user.UserName));
        claims.Add(new Claim(SystemConstants.Claims.UserId, user.Id));
        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        claims.Add(new Claim(ClaimTypes.Email, user.Email));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        claims.Add(new Claim(SystemConstants.Claims.Roles, string.Join("", roles)));
        claims.Add(new Claim(SystemConstants.Claims.Permissions,JsonSerializer.Serialize(permissions)));

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(sub);
        context.IsActive = user != null;
    }
}
