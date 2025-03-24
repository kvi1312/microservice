using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Contracts.Common.Interfaces;

public interface IRepositoryBaseAsync<T, TKey, TContext> : IRepositoryQueryBase<T, TKey, TContext> where T : EntityBase<TKey> where TContext : DbContext
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