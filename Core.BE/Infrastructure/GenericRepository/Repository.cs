using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Emeint.Core.Managers;

namespace mConnect.Framework.Infrastructure.GenericRepository
{
    public class Repository<TContext, TEntity> : IRepository<TEntity>, IDisposable
        where TContext : DbContext, new()
        where TEntity : class
    {
        private readonly TContext _dataContext;

        public Repository(TContext dataContext)
        {
            _dataContext = dataContext;
        }

        public DbContext GetCurrentContext { get { return _dataContext; } }

        public void Dispose()
        {
            if (_dataContext != null)
                _dataContext.Dispose();
            GC.SuppressFinalize(this);
        }


        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return _dataContext.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefault(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dataContext.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dataContext.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public List<TEntity> GetList()
        {
            return _dataContext.Set<TEntity>()
                .AsNoTracking()
                .ToList();
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            if (typeof(TEntity).IsSubclassOf(typeof(MultiTenant.MultiTenant)))
            {

                var currentServiceId = Identity.Instance.ServiceId;

                var tableName = _dataContext.GetTableName(typeof(TEntity));

                var q1 = _dataContext
                 .Set<TEntity>()
                 .SqlQuery(string.Format("SELECT * FROM {0} WHERE [ServiceId] = {1}", tableName, currentServiceId));

                var q2 = q1.Where(predicate.Compile());

                var q3 = q2.ToList();
                return q3;
            }
            return _dataContext.Set<TEntity>()
                .Where(predicate)
                .ToList();
        }

        public List<TEntity> GetList<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy)
        {
            return _dataContext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(orderBy)
                .ToList();
        }

        public List<TEntity> GetList<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy, int pageNo, int pageSize)
        {
            return _dataContext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(orderBy)
                .Skip(pageNo * pageSize)
                .Take(pageSize).ToList();

        }

        public async Task<List<TEntity>> GetListAsync()
        {
            return await _dataContext.Set<TEntity>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate != null)
                return await _dataContext.Set<TEntity>()
                    .AsNoTracking()
                    .Where(predicate)
                    .ToListAsync();
            return await GetListAsync();

        }

        public async Task<List<TEntity>> GetListAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy)
        {
            return await _dataContext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(orderBy)
                .ToListAsync();
        }

        public async Task<List<TEntity>> GetListAsync<TKey>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TKey>> orderBy, int pageNo, int pageSize)
        {
            return await _dataContext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(orderBy)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public TEntity Find(params object[] keyValues)
        {
            return _dataContext.Set<TEntity>()
                .Find(keyValues);
        }

        public async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await _dataContext.Set<TEntity>()
                .FindAsync(keyValues);
        }

        public TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return _dataContext.Set<TEntity>()
                .FirstOrDefault(predicate);
        }

        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dataContext.Set<TEntity>()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dataContext.Set<TEntity>()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public TEntity Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var baseType = typeof(TEntity).BaseType;
            if (baseType != null && baseType == typeof(MultiTenant.MultiTenant))
            {
                var serviceId = Identity.Instance.ServiceId;
                entity.GetType().GetProperty("ServiceId").SetValue(entity, serviceId);

                baseType.GetProperty("ServiceId", typeof(int)).SetValue(entity, serviceId);
            }

            return _dataContext.Set<TEntity>().Add(entity);

        }

        public List<TEntity> AddList(List<TEntity> entities)
        {
            //https://efbulkinsert.codeplex.com/
            //http://www.entityframeworktutorial.net/EntityFramework6/addrange-removerange.aspx

            if (entities == null || !entities.Any())
                throw new ArgumentNullException("entities");

            var baseType = typeof(TEntity).BaseType;
            if (baseType != null && baseType == typeof(MultiTenant.MultiTenant))
            {
                var serviceId = Identity.Instance.ServiceId;
                entities.ForEach(entity =>
                {
                    entity.GetType().GetProperty("ServiceId").SetValue(entity, serviceId);

                    baseType.GetProperty("ServiceId", typeof(int)).SetValue(entity, serviceId);
                });
            }

            return _dataContext.Set<TEntity>().AddRange(entities).ToList();
        }

        public TEntity Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //TODO: need more testing by taham
            //// if entity type is a database type (DynamicProxies), then you don't need to modify it's EntityState
            //// it's EntityState is already modified because it is tracked
            //if (entity.GetType().BaseType != null
            //    && entity.GetType().Namespace == "System.Data.Entity.DynamicProxies")
            //{
            //    if (_dataContext.Entry(entity).State == EntityState.Detached)
            //    {
            //        _dataContext.Entry(entity).State = EntityState.Modified;
            //    }
            //    return entity;
            //}


            var primaryKeyName = _dataContext.GetPrimaryKey(typeof(TEntity)).Name;

            var primaryKeyValue = entity.GetType().GetProperty(primaryKeyName).GetValue(entity);

            var entityFromDb = Find(primaryKeyValue);


            if (entityFromDb == null)
                throw new Exception("Entity Not Found with the specified params");

            var baseType = typeof(TEntity).BaseType;

            if (baseType != null && baseType == typeof(MultiTenant.MultiTenant))
            {
                var serviceId = Identity.Instance.ServiceId;
                entity.GetType().GetProperty("ServiceId", typeof(int)).SetValue(entity, serviceId);

                baseType.GetProperty("ServiceId", typeof(int)).SetValue(entity, serviceId);
            }

            _dataContext.Entry(entityFromDb).CurrentValues.SetValues(entity);
            _dataContext.Entry(entityFromDb).State = EntityState.Modified;


            return entity;
        }

        public List<TEntity> UpdateList(List<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            //TODO: need more testing by taham
            //// if entities type is a database type (DynamicProxies), then you don't need to modify their EntityState.
            //// their EntityState are already modified because they are tracked.
            //if (entities.Any()
            //    && entities.First().GetType().BaseType != null
            //    && entities.First().GetType().Namespace == "System.Data.Entity.DynamicProxies")
            //{
            //    //foreach (var entity in entities.Where(entity => _dataContext.Entry(entity).State == EntityState.Detached))
            //    //{
            //    //    _dataContext.Entry(entity).State = EntityState.Modified;
            //    //}
            //    Parallel.ForEach(entities.Where(entity => _dataContext.Entry(entity).State == EntityState.Detached),
            //        entity =>
            //        {
            //            _dataContext.Entry(entity).State = EntityState.Modified;
            //        });

            //    return entities;
            //}

            var primaryKeyName = _dataContext.GetPrimaryKey(typeof(TEntity)).Name;

            var propertyInfo = typeof(TEntity).GetProperty(primaryKeyName);
            var entitiesIds = entities.Select(e => propertyInfo.GetValue(e)).ToList();

            var commaSeperatedIds = string.Join(",", entitiesIds);
            var currentServiceId = Identity.Instance.ServiceId;
            var tableName = _dataContext.GetTableName(typeof(TEntity));


            string query;
            if (typeof(TEntity).IsSubclassOf(typeof(MultiTenant.MultiTenant)))
            {
                query = string.Format("SELECT * FROM {0} WHERE [ServiceId] = {1} AND {2} IN ({3})", tableName,
                    currentServiceId, primaryKeyName, commaSeperatedIds);
            }
            else
            {
                query = string.Format("SELECT * FROM {0} WHERE {1} IN ({2})", tableName, primaryKeyName,
                    commaSeperatedIds);
            }

            var dbEntities = _dataContext.Set<TEntity>()
                .SqlQuery(query)
                .ToList();

            // https://msdn.microsoft.com/en-us/library/dd460720(v=vs.110).aspx

            var o = new object();
            try
            {
                Parallel.ForEach(entitiesIds, id =>
                {
                    var entity = entities.FirstOrDefault(e => propertyInfo.GetValue(e).ToString() == id.ToString());

                    var dbEntity = dbEntities.FirstOrDefault(e => propertyInfo.GetValue(e).ToString() == id.ToString());

                    if (dbEntity != null && entity != null)
                    {
                        lock (o)
                        {
                            _dataContext.Entry(dbEntity).CurrentValues.SetValues(entity);
                            _dataContext.Entry(dbEntity).State = EntityState.Modified;
                        }
                    }
                });
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    LogsManager.Error(ex.Message, ex);
                    return false; // Let anything else stop the application.
                });
            }



            return entities;
        }

        public TEntity UpdateCascade(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var primaryKeyName = _dataContext.GetPrimaryKey(typeof(TEntity)).Name;

            var primaryKeyValue = entity.GetType().GetProperty(primaryKeyName).GetValue(entity);

            var entityFromDb = Find(primaryKeyValue);


            if (entityFromDb == null)
            {
                throw new Exception("Entity Not Found with the specified params");
            }


            _dataContext.UpdateDbEntity(entityFromDb, entity);

            return entity;
        }

        public void Delete(params object[] keyValues)
        {
            var entity = Find(keyValues);
            if (entity != null)
                _dataContext.Set<TEntity>().Remove(entity);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = GetList(predicate);
            if (entities.Any())
                _dataContext.Set<TEntity>().RemoveRange(entities);
        }

        public int Max(Func<TEntity, int> selector)
        {
            return _dataContext.Set<TEntity>()
                .Max(selector);
        }

    }
}
