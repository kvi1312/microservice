using AutoMapper;
using Dapper;
using IdentityServer.Persistence;
using Microservice.IDP.Infrastructure.Domain;
using Microservice.IDP.Infrastructure.Entities;
using Microservice.IDP.Infrastructure.ViewModels;
using Microservices.IDP.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Microservice.IDP.Infrastructure.Repositories;

public class PermissionRepository : RepositoryBase<Permission, long>, IPermissionRepository
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    public PermissionRepository(IdentityContext dbContext, IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper) : base(dbContext, unitOfWork)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId);
        var result = await QueryAsync<PermissionViewModel>("Get_Permission_ByRoleId", parameters);
        return result;
    }

    public async Task<PermissionViewModel?> CreatePermission(string roleId, PermissionAddModel model)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", model.Function, DbType.String);
        parameters.Add("@command", model.Command, DbType.String);
        parameters.Add("@newID", model.Command, DbType.Int64, direction: ParameterDirection.Output);
        var result = await ExecuteAsync("Create_Permissions", parameters);

        if (result <= 0) return null;

        var newId = parameters.Get<long>("@newID");

        return new PermissionViewModel()
        {
            Id = newId,
            Function = model.Function,
            Command = model.Command,
            RoleId = roleId,
        };
    }

    public Task UpdatePermissionByRoleId(string roleId, IEnumerable<PermissionAddModel> permissionCollection)
    {
        var dt = new DataTable();
        dt.Columns.Add("RoleId", typeof(string));
        dt.Columns.Add("Function", typeof(string));
        dt.Columns.Add("Command", typeof(string));

        foreach (var item in permissionCollection)
        {
            dt.Rows.Add(roleId, item.Function, item.Command);
        }

        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@permissions", dt.AsTableValuedParameter("dbo.Permission"));
        return ExecuteAsync("Update_Permissions_ByRole", parameters);
    }

    public Task DeletePermission(string roleId, string function, string command)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", function, DbType.String);
        parameters.Add("@command", command, DbType.String);
        return ExecuteAsync("Delete_Permissions", parameters);
    }

    public async Task<IEnumerable<PermissionUserViewModel>> GetPermissionByUser(User user)
    {
        var currentUserRoles = await _userManager.GetRolesAsync(user);
        var query = FindAll(false)
            .Where(x => currentUserRoles.Contains(x.RoleId))
            .Select(x => new Permission(x.Function, x.Command, x.RoleId));

        var result = _mapper.Map<IEnumerable<PermissionUserViewModel>>(query);
        return result;
    }
}