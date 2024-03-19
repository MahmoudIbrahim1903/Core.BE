using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace mConnect.Framework.Infrastructure.GenericRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        int Max(Func<TEntity, int> selector);

        List<TEntity> GetList();
        List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate);
        List<TEntity> GetList<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy);
        List<TEntity> GetList<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, int pageNo, int pageSize);

        Task<List<TEntity>> GetListAsync();
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetListAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy);
        Task<List<TEntity>> GetListAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, int pageNo, int pageSize);

        TEntity Find(params object[] keyValues);
        Task<TEntity> FindAsync(params object[] keyValues);

        TEntity First(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        TEntity Add(TEntity entity);
        List<TEntity> AddList(List<TEntity> entities);

        TEntity Update(TEntity entity);
        List<TEntity> UpdateList(List<TEntity> entities);
        TEntity UpdateCascade(TEntity entity);

        void Delete(params object[] keyValues);

        void Delete(Expression<Func<TEntity, bool>> predicate);

        DbContext GetCurrentContext { get; }
    }
}
