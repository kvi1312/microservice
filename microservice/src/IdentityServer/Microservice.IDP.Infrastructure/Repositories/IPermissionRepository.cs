using Microservice.IDP.Infrastructure.Domain;
using Microservice.IDP.Infrastructure.Entities;
using Microservices.IDP.Infrastructure.ViewModels;

namespace Microservice.IDP.Infrastructure.Repositories;

public interface IPermissionRepository : IRepositoryBase<Permission, long>
{
    Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId);
    void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissionCollection, bool trackChanges);
}