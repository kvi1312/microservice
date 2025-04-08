using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Contracts.Common.Interfaces;

public interface IRepositoryBaseAsync<T, TKey> : IRepositoryQueryBase<T, TKey> where T : EntityBase<TKey>
{
    Task<TKey> CreateAsync(T entity);
    Task<IList<TKey>> CreateListAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateListAsync(IEnumerable<T> entities);
    Task DeleteAsync(T entity);
    Task DeleteListAsync(IEnumerable<T> entities);
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransaction();
    Task EndTransactionAsync();
    Task RollbackTransactionAsync();
}

public interface IRepositoryBaseAsync<T, K, TContext> : IRepositoryBaseAsync<T, K>
    where T : EntityBase<K>
    where TContext : DbContext
{
}