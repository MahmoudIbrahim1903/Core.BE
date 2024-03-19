using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.SeedWork;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Emeint.Core.BE.Infrastructure.Repositories
{
    public class BaseRepository<TEntity, TContext> : IRepository<TEntity>, IDisposable
        where TEntity : class
        where TContext : DbContext
    {
        protected readonly TContext _context;

        //public IUnitOfWork UnitOfWork
        //{
        //    get
        //    {
        //        return _context;
        //    }
        //}

        //public BaseRepository(DbContext context)
        //{
        //    _context = context;
        //}

        public BaseRepository(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>()
                /*.AsNoTracking()*/;
        }

        public virtual TEntity Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _context.Set<TEntity>().Add(entity).Entity;
        }

        public virtual async Task<TEntity> GetAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return entity;
        }

        //public virtual TEntity GetByCode<T>(string code)
        //{
        //    var entity = _context.Set<TEntity>().FirstOrDefault(m => m.Code == code);
        //    if (entity == null)
        //        throw new ArgumentNullException(nameof(entity));
        //    return entity;
        //}

        public virtual void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            // await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed throught the DbContext will be commited
            //test comment

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void SaveEntities(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            // await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed throught the DbContext will be commited
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Delete(TEntity entity)
        {
            _context.Remove(entity);
        }

        public Task<List<TEntity>> AddBulkAsync(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> UpdateBulkAsync(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}