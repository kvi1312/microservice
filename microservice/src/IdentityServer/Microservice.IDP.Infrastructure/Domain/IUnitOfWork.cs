namespace Microservice.IDP.Infrastructure.Domain;

public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
}