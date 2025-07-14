using IdentityServer.Common.Domain;
using IdentityServer.Entities;
using IdentityServer.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityServer.Repositories;

// Every controller or service layer just need to DI this IRepositoryManager -> can use every repository, Ex : UserRepository, RoleRepository...
public class RepositoryManager(
    IUnitOfWork unitOfWork,
    IdentityContext dbContext,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager)
    : IRepositoryManager
{
    public UserManager<User> UserManager { get; } = userManager;
    
    public RoleManager<IdentityRole> RoleManager { get; } = roleManager;


    public async Task<int> SaveAsync() => await unitOfWork.CommitAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync() => dbContext.Database.BeginTransactionAsync();

    public Task EndTransactionAsync() => dbContext.Database.CommitTransactionAsync();

    public void RollbackTransaction() => dbContext.Database.RollbackTransaction();
}