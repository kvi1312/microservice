using Microservice.IDP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microservice.IDP.Infrastructure.Repositories;

public interface IRepositoryManager
{
    IPermissionRepository PermissionRepository { get; }
    Task<int> SaveAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task EndTransactionAsync();
    void RollbackTransaction();
    UserManager<User> UserManager { get; }
    RoleManager<IdentityRole> RoleManager { get; }
}