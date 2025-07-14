using IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityServer.Repositories;

public interface IRepositoryManager 
{
    UserManager<User> UserManager { get; }
    RoleManager<IdentityRole> RoleManager { get; }

    Task<int> SaveAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task EndTransactionAsync();
    void RollbackTransaction();
    
}