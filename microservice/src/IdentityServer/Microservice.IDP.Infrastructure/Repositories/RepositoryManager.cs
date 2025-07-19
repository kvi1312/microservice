using IdentityServer.Persistence;
using Microservice.IDP.Infrastructure.Domain;
using Microservice.IDP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microservice.IDP.Infrastructure.Repositories;

// Every controller or service layer just need to DI this IRepositoryManager -> can use every repository, Ex : UserRepository, RoleRepository...
public class RepositoryManager : IRepositoryManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IdentityContext _dbContext;
    private readonly Lazy<IPermissionRepository> _permissionRepository;
    public UserManager<User> UserManager { get; }
    public RoleManager<IdentityRole> RoleManager { get; }

    public RepositoryManager(IUnitOfWork unitOfWork,
        IdentityContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        UserManager = userManager;
        RoleManager = roleManager;
        _permissionRepository =
            new Lazy<IPermissionRepository>(() =>
                new PermissionRepository(_dbContext,
                    _unitOfWork)); //  applying lazyload, actual inuse when PermissionRepository is called
    }

    public IPermissionRepository PermissionRepository => _permissionRepository.Value;
    public async Task<int> SaveAsync() => await _unitOfWork.CommitAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync() => _dbContext.Database.BeginTransactionAsync();

    public Task EndTransactionAsync() => _dbContext.Database.CommitTransactionAsync();

    public void RollbackTransaction() => _dbContext.Database.RollbackTransaction();
}