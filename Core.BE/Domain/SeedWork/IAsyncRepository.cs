using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Domain.SeedWork
{
    public interface IAsyncRepository<TEntity> where TEntity : class
    {
        //IUnitOfWork UnitOfWork { get; }

        IQueryable<TEntity> GetAll();
        Task<TEntity> AddAsync(TEntity entity);
        Task<List<TEntity>> AddBulkAsync(List<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task<List<TEntity>> UpdateBulkAsync(List<TEntity> entities);
        Task<TEntity> GetAsync(int id);

        //T GetByCode<T>(string code);
        Task DeleteAsync(TEntity entity);
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}