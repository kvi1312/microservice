using IdentityServer.Persistence;

namespace Microservice.IDP.Infrastructure.Domain;

public class UnitOfWork : IUnitOfWork
{
    private readonly IdentityContext _dbContext;

    public UnitOfWork(IdentityContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task<int> CommitAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}