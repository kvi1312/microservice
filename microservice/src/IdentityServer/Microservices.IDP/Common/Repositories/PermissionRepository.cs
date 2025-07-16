using IdentityServer.Common.Domain;
using IdentityServer.Entities;
using IdentityServer.Persistence;

namespace IdentityServer.Common.Repositories;

public class PermissionRepository : RepositoryBase<Permission, long>, IPermissionRepository
{
    public PermissionRepository(IdentityContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public Task<IEnumerable<Permission>> GetPermissionsByRole(string roleId, bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissionCollection, bool trackChanges)
    {
        throw new NotImplementedException();
    }
}