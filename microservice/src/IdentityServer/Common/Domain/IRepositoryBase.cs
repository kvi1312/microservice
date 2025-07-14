﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityServer.Common.Domain;

public interface IRepositoryBase<T, K> where T : EntityBase<K>
{
    # region query
    
    IQueryable<T> FindAll(bool trackChanges = false);
    IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties);
    
    Task<T?> GetByIdAsync(K id);
    Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties);
    
    #endregion
    
    #region Action
    
    Task<K> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task UpdateListAsync(IEnumerable<T> entities);
    Task DeleteAsync(T entity);
    Task DeleteListAsync(IEnumerable<T> entities);
    
    #endregion

    #region Transaction Action
    
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction>  BeginTransactionAsync();
    Task EndTransactionAsync();
    Task RollbackTransactionAsync();

    #endregion
    
}