namespace IdentityServer.Common.Domain;

public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
}