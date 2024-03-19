using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Domain.SeedWork
{
    public interface IRepository<TEntity> where TEntity : class
    {
        //IUnitOfWork UnitOfWork { get; }

        IQueryable<TEntity> GetAll();
        TEntity Add(TEntity entity);
        Task<List<TEntity>> AddBulkAsync(List<TEntity> entities);
        void Update(TEntity entity);
        Task<List<TEntity>> UpdateBulkAsync(List<TEntity> entities);
        Task<TEntity> GetAsync(int id);

        //T GetByCode<T>(string code);
        void Delete(TEntity entity);

        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));

        void SaveEntities(CancellationToken cancellationToken = default(CancellationToken));
    }
}