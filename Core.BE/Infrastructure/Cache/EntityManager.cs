using Emeint.Common.Infrastructure.GenericRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Emeint.Core.BE.Infrastructure.Cache
{
    /// <summary>
    /// A singleton class that manage all entity opertaions to the database and memory cash
    /// </summary>
    public sealed class EntityManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheProvider _cache;

        private static readonly object Locker = new object();
        private readonly static Hashtable EntityManagerTable;


        #region Singleton
        static EntityManager()
        {
            EntityManagerTable = new Hashtable();
        }
        private EntityManager(IUnitOfWork unitOfWork)
            : this(CacheProvider.Instance, unitOfWork) { }

        private EntityManager(ICacheProvider cacheProvider, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _cache = cacheProvider;
        }

        /// <summary>
        /// A singleton instance of the EntityManager class
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public static EntityManager Instance(IUnitOfWork unitOfWork)
        {
            lock (Locker)
            {
                var unitOfWorkKey = unitOfWork.GetType().GenericTypeArguments[0].FullName;
                if (!EntityManagerTable.ContainsKey(unitOfWorkKey))
                    EntityManagerTable.Add(unitOfWorkKey, new EntityManager(unitOfWork));

                return (EntityManager)EntityManagerTable[unitOfWorkKey];
            }
        }
        #endregion

        /// <summary>
        /// clear cache of TEntity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        public void Invalidate<TEntity>()
        {
            var key = typeof(TEntity).Name;
            _cache.Invalidate(key);
        }



        /// <summary>
        /// get entity from data base by predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public List<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate = null, bool cache = true) where TEntity : class
        {
            //to prevent null exception
            if (predicate == null)
            {
                predicate = o => true;
            }


            if (cache)
            {
                var key = typeof(TEntity).Name;
                var cashedObject = _cache.Get(key) as List<TEntity>;


                if (cashedObject == null)
                {
                    cashedObject = _unitOfWork.Repository<TEntity>().GetList(o => true);
                    _cache.Set(key, cashedObject, cacheTime: 30);
                }

                return cashedObject.Where(predicate.Compile()).ToList();
            }
            var dbObject = _unitOfWork.Repository<TEntity>().GetList(predicate);
            return dbObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValues"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity GetObjectById<TEntity>(params object[] keyValues) where TEntity : class
        {
            //var key = typeof(TEntity).Name;
            return _unitOfWork.Repository<TEntity>().Find(keyValues);
        }

       
        /// <summary>
        /// insert the given entity into the database
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            var addedEntity = _unitOfWork.Repository<TEntity>().Add(entity);

            _unitOfWork.SaveChanges();

            Invalidate<TEntity>();
            return addedEntity;
        }


        /// <summary>
        /// insert the given list of entities into database
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<TEntity> AddList<TEntity>(List<TEntity> entities) where TEntity : class
        {
            //var addedEntites = entities.Select(entity => _unitOfWork.Repository<TEntity>().Add(entity)).ToList();
            var addedEntites = _unitOfWork.Repository<TEntity>().AddList(entities);

            _unitOfWork.SaveChanges();

            Invalidate<TEntity>();
            return addedEntites;
        }


        /// <summary>
        /// Update the selected by params database entity by the given entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyValues">the params by which </param>
        /// <returns>The updated database entity</returns>
        public TEntity Update<TEntity>(TEntity entity, params object[] keyValues) where TEntity : class
        {
            var updatedEntity = _unitOfWork.Repository<TEntity>().Update(entity);

            _unitOfWork.SaveChanges();

            Invalidate<TEntity>();

            return updatedEntity;
        }


        /// <summary>
        /// Update a List of entity from database with the given list
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public void UpdateList<TEntity>(List<TEntity> entities) where TEntity : class
        {
            _unitOfWork.Repository<TEntity>().UpdateList(entities);

            _unitOfWork.SaveChanges();

            Invalidate<TEntity>();
        }


        /// <summary>
        /// Update the database entity and its children collections
        /// </summary>
        /// <typeparam name="TEntity">
        /// the parent object to be updated.
        /// the parent object and its children collections item must have a primary key of type integar.
        /// </typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity UpdateCascade<TEntity>(TEntity entity) where TEntity : class
        {
            var updatedEntity = _unitOfWork.Repository<TEntity>().UpdateCascade(entity);
            _unitOfWork.SaveChanges();

            Invalidate<TEntity>();

            return updatedEntity;
        }


        /// <summary>
        /// Delete a database entity selected by keyValues params
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public bool Delete<TEntity>(params object[] keyValues) where TEntity : class
        {
            _unitOfWork.Repository<TEntity>().Delete(keyValues);

            var status = _unitOfWork.SaveChanges();

            Invalidate<TEntity>();
            return status > 0;

        }


        /// <summary>
        /// Delete a database entities selected by predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public bool Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            _unitOfWork.Repository<TEntity>().Delete(predicate);

            var status = _unitOfWork.SaveChanges();

            Invalidate<TEntity>();
            return status > 0;

        }

    }
}