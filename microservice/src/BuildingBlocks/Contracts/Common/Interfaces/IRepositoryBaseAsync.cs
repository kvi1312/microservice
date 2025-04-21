using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Contracts.Common.Interfaces;

public interface IRepositoryBaseAsync<T, TKey> : IRepositoryQueryBase<T, TKey> where T : EntityBase<TKey>
{
    void Create(T entity);
    Task<TKey> CreateAsync(T entity);
    IList<TKey> CreateList(IEnumerable<T> entities);
    Task<IList<TKey>> CreateListAsync(IEnumerable<T> entities);
    void Update(T entity);
    Task UpdateAsync(T entity);
    void UpdateList(IEnumerable<T> entities);
    Task UpdateListAsync(IEnumerable<T> entities);
    void Delete(T entity);
    Task DeleteAsync(T entity);
    void DeleteList(IEnumerable<T> entities);
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