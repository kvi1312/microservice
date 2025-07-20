using Microservice.IDP.Infrastructure.Domain;
using Microservice.IDP.Infrastructure.Entities;
using Microservices.IDP.Infrastructure.ViewModels;

namespace Microservice.IDP.Infrastructure.Repositories;

public interface IPermissionRepository : IRepositoryBase<Permission, long>
{
    Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId);
    Task<PermissionViewModel?> CreatePermission(string roleId, PermissionAddModel model);
    Task DeletePermission(string roleId, string function, string command);
    Task UpdatePermissionByRoleId(string roleId, IEnumerable<PermissionAddModel> permissionCollection);
}