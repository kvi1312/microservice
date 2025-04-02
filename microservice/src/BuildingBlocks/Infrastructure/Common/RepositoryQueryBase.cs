using System.Linq.Expressions;
using Contracts.Common.Interfaces;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common;

public class RepositoryQueryBase<T, TKey, TContext> : IRepositoryQueryBase<T, TKey, TContext> where TContext : DbContext where T : EntityBase<TKey>
{
    private readonly TContext _dbContext;

    public RepositoryQueryBase(TContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IQueryable<T> FindAll(bool trackChange = false) =>
        !trackChange ? _dbContext.Set<T>().AsNoTracking() : _dbContext.Set<T>();

    public IQueryable<T> FindAll(bool trackChange = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindAll(trackChange);
        items = includeProperties.Aggregate(items, (current, properties) => current.Include(properties));
        return items;
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChange = false) =>
        !trackChange
            ? _dbContext.Set<T>().Where(expression).AsNoTracking()
            : _dbContext.Set<T>().Where(expression);

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expressions, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperty)
    {
        var items = FindByCondition(expressions, trackChanges);
        items = includeProperty.Aggregate(items, (current, property) => current.Include(property));
        return items;
    }

    public async Task<T?> GetByIdAsync(TKey id)
    {
        return await FindByCondition(x => x.Id != null && x.Id.Equals(id)).FirstOrDefaultAsync();
    }

    public async Task<T?> GetByIdAsync(TKey id, params Expression<Func<T, object>>[] includeProperties)
    {
        return await FindByCondition(x => x.Id != null && x.Id.Equals(id), trackChanges: false, includeProperties).FirstOrDefaultAsync();
    }
}