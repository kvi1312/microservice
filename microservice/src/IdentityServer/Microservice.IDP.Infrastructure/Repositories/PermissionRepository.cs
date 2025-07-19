using IdentityServer.Persistence;
using Microservice.IDP.Infrastructure.Domain;
using Microservice.IDP.Infrastructure.Entities;
using Dapper;
using Microservices.IDP.Infrastructure.ViewModels;

namespace Microservice.IDP.Infrastructure.Repositories;

public class PermissionRepository : RepositoryBase<Permission, long>, IPermissionRepository
{
    public PermissionRepository(IdentityContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId);
        var result = await QueryAsync<PermissionViewModel>("Get_Permission_ByRoleId", parameters);
        return result;
    }

    public void UpdatePermissionByRoleId(string roleId, IEnumerable<Permission> permissionCollection, bool trackChanges)
    {
        throw new NotImplementedException();
    }
}