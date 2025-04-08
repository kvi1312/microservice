using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Contracts.Common.Interfaces;

    // TKey ref for primary key of T
    // IRepositoryQueryBase - Handle for getting Data; IRepositoryBaseAsync - CRUD data
    public interface IRepositoryQueryBase<T, in TKey> where T : EntityBase<TKey>
    {
        IQueryable<T> FindAll(bool trackChange = false);

        IQueryable<T> FindAll(bool trackChange = false, params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChange = false);

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expressions, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperty);

        Task<T?> GetByIdAsync(TKey id);

        Task<T?> GetByIdAsync(TKey id, params Expression<Func<T, object>>[] includeProperties);
        
    }
    
    public interface IRepositoryQueryBase<T, K, TContext> : IRepositoryQueryBase<T, K> where T : EntityBase<K>
        where TContext : DbContext
    {
    
    }

