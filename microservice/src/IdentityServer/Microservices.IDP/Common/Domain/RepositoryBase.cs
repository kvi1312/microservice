﻿using System.Linq.Expressions;
using IdentityServer.Persistence;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityServer.Common.Domain;

public class RepositoryBase<T, K> : IRepositoryBase<T, K> where T : EntityBase<K>
{
    private readonly IdentityContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public RepositoryBase(IdentityContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public IQueryable<T> FindAll(bool trackChanges = false) =>
        !trackChanges ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

    public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindAll(trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
        => !trackChanges ? _dbContext.Set<T>().Where(expression).AsNoTracking() : _dbContext.Set<T>().Where(expression);

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindByCondition(expression, trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }

    public Task<T?> GetByIdAsync(K id) => FindByCondition(x => x.Equals(id)).FirstOrDefaultAsync();

    public Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties)
        => FindByCondition(x => x.Equals(id), trackChanges: false, includeProperties).FirstOrDefaultAsync();

    public async Task<K> CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _unitOfWork.CommitAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Unchanged) return;
        
        T existedItem = await _dbContext.Set<T>().FindAsync(entity.Id);
        _dbContext.Entry(existedItem).CurrentValues.SetValues(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateListAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().UpdateRange(entities);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteListAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        await _unitOfWork.CommitAsync();
    }

    public Task<int> SaveChangesAsync() => _unitOfWork.CommitAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync() 
        => _dbContext.Database.BeginTransactionAsync();

    public async Task EndTransactionAsync()
    {
        await SaveChangesAsync();
        await _dbContext.Database.CommitTransactionAsync();
    }

    public Task RollbackTransactionAsync() 
        => _dbContext.Database.RollbackTransactionAsync();
}