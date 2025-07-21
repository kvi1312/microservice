using AutoMapper;
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
    private readonly IMapper _mapper;

    public RepositoryManager(IUnitOfWork unitOfWork,
        IdentityContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        UserManager = userManager;
        RoleManager = roleManager;

        //  applying lazyload, actual inuse when PermissionRepository is called
        _permissionRepository =
            new Lazy<IPermissionRepository>(() =>
                new PermissionRepository(_dbContext,
                    _unitOfWork, userManager, _mapper));
        _mapper = mapper;
    }

    public IPermissionRepository PermissionRepository => _permissionRepository.Value;
    public async Task<int> SaveAsync() => await _unitOfWork.CommitAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync() => _dbContext.Database.BeginTransactionAsync();

    public Task EndTransactionAsync() => _dbContext.Database.CommitTransactionAsync();

    public void RollbackTransaction() => _dbContext.Database.RollbackTransaction();
}