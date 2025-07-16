using IdentityServer.Common.Domain;
using IdentityServer.Entities;

namespace IdentityServer.Common.Repositories;

public interface IPermissionRepository : IRepositoryBase<Permission, long>
{
    Task<IEnumerable<Permission>> GetPermissionsByRole(string roleId, bool trackChanges);
    void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissionCollection, bool trackChanges);
}