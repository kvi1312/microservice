using System.Linq.Expressions;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Contracts.Common.Interfaces;

public interface IRepositoryQueryBase<T, K, TContext> where T : EntityBase<K> where TContext : DbContext
{
    IQueryable<T> FindAll(bool trackChange = false);

    IQueryable<T> FindAll(bool trackChange = false, params Expression<Func<T, object>>[] includeProperties);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChange = false);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expressions, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperty);

    Task<T?> GetByIdAsync(K id);

    Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties);
}

public interface IRepositoryBaseAsync<T, K, TContext> : IRepositoryQueryBase<T, K, TContext> where T : EntityBase<K> where TContext : DbContext
{
    Task<K> CreateAsync(T entity);

    Task<IList<K>> CreateListAsync(IEnumerable<T> entities);

    Task UpdateAsync(T entity);

    Task UpdateListAsync(IEnumerable<T> entities);

    Task DeleteAsync(T entity);

    Task DeleteListAsync(IEnumerable<T> entities);

    Task<int> SaveChangesAsync();

    Task<IDbContextTransaction> BeginTransaction();

    Task EndTransactionAsync();

    Task RollbackTransactionAsync();
}