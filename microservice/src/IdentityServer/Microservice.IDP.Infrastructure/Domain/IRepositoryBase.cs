using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microservice.IDP.Infrastructure.Domain;

public interface IRepositoryBase<T, K> where T : EntityBase<K>
{
    #region query

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
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task EndTransactionAsync();
    Task RollbackTransactionAsync();

    #endregion

    #region Dapper
    /// <summary>
    /// Using TModel to prevent auto mapping feature of dapper return unwanted field
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <param name="commandType"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TModel>> QueryAsync<TModel>(string sql,
                                                   object? param,
                                                   CommandType? commandType,
                                                   IDbTransaction? transaction,
                                                   int? commandTimeout) where TModel : EntityBase<K>;

    Task<TModel> QueryFirstOrDefaultAsync<TModel>(string sql,
                                                   object? param,
                                                   CommandType? commandType,
                                                   IDbTransaction? transaction,
                                                   int? commandTimeout) where TModel : EntityBase<K>;

    Task<TModel> QuerySingleAsync<TModel>(string sql,
                                               object? param,
                                               CommandType? commandType,
                                               IDbTransaction? transaction,
                                               int? commandTimeout) where TModel : EntityBase<K>;

    Task<int> ExecuteAsync<TModel>(string sql,
                                           object? param,
                                           CommandType? commandType,
                                           IDbTransaction? transaction,
                                           int? commandTimeout);
    #endregion
}