using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace mConnect.Framework.Infrastructure.GenericRepository
{
    /// <summary>
    /// Helper class to add some functionality to DbContext
    /// </summary>
    public static class DataContextHelper
    {
        /// <summary>
        /// Get the primary key object of a TEntity type in the context
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static EdmMember GetPrimaryKey<TEntity>(this DbContext context)
        {
            return context.GetTable(typeof(TEntity)).ElementType.KeyMembers.First();
        }

        /// <summary>
        /// Get the primary key object of a TEntity type in the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EdmMember GetPrimaryKey(this DbContext context, Type type)
        {
            return context.GetTable(type).ElementType.KeyMembers.First();
        }

        /// <summary>
        /// Get the database table name of a type in the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(this DbContext context, Type type)
        {
            var table = context.GetTable(type);
            return (string)table.MetadataProperties["Table"].Value ?? String.Format("[{0}].[{1}]", table.Schema, table.Name);
        }

        /// <summary>
        /// Get the database table of a type in the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EntitySet GetTable(this DbContext context, Type type)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                .GetItems<EntityType>(DataSpace.OSpace)
                .Single(e => objectItemCollection.GetClrType(e) == type);

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                .Single()
                .EntitySetMappings
                .Single(s => s.EntitySet == entitySet);

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;
            return table;
            // Return the table name from the storage entity set

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityFromDb"></param>
        /// <param name="entity"></param>
        public static void UpdateDbEntity(this DbContext context, object entityFromDb, object entity)
        {
            var baseType = entity.GetType().BaseType;
            if (baseType != null && baseType == typeof(MultiTenant.MultiTenant))
            {
                var serviceId = Identity.Instance.ServiceId;

                entity.GetType().GetProperty("ServiceId", typeof(int)).SetValue(entity, serviceId);

                baseType.GetProperty("ServiceId", typeof(int)).SetValue(entity, serviceId);
            }

            context.Entry(entityFromDb).CurrentValues.SetValues(entity);
            context.Entry(entityFromDb).State = EntityState.Modified;


            var entityCollections =
                entity.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.Namespace == "System.Collections.Generic")
                    .ToList();

            var entityFromDbCollections =
                entityFromDb.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.Namespace == "System.Collections.Generic")
                    .ToList();



            foreach (var collection in entityFromDbCollections)
            {
                var dbCollection = collection.GetValue(entityFromDb) as IEnumerable<object>;
                if (dbCollection != null)
                {
                    var dbCollectionItemType = TypeSystem.GetElementType(dbCollection.GetType());

                    var primaryKeyName = context.GetPrimaryKey(dbCollectionItemType).Name;

                    var dbCollectionIds = dbCollection.Select(i => (int)dbCollectionItemType.GetProperty(primaryKeyName).GetValue(i));


                    var entityCollectionMatch = entityCollections.Find(e => e.Name == collection.Name);
                    if (entityCollectionMatch != null)
                    {
                        var entityCollection = entityCollectionMatch.GetValue(entity) as IEnumerable<object>;

                        if (entityCollection != null)
                        {
                            var entityCollectionItemType = TypeSystem.GetElementType(entityCollection.GetType());

                            var entityCollectionIds =
                                entityCollection.Select(e => (int)entityCollectionItemType.GetProperty(primaryKeyName).GetValue(e));

                            var toBeDeletedIds = dbCollectionIds.Except(entityCollectionIds);

                            var toBeDeletedDBEntities = dbCollection.Where(
                                i => toBeDeletedIds.Contains((int)dbCollectionItemType.GetProperty(primaryKeyName).GetValue(i))).ToList();
                            toBeDeletedDBEntities.ForEach(e => context.Entry(e).State = EntityState.Deleted);

                            var toBeModeifiedIds = dbCollectionIds.Intersect(entityCollectionIds);
                            var toBeModeifiedDBEntities =
                                dbCollection.Where(
                                    i =>
                                        toBeModeifiedIds.Contains((int)dbCollectionItemType.GetProperty(primaryKeyName).GetValue(i)))
                                    .ToList();

                            toBeModeifiedDBEntities.ForEach(dbEntity =>
                            {
                                var ent =
                                    entityCollection.FirstOrDefault(
                                        i =>
                                            (int)entityCollectionItemType.GetProperty(primaryKeyName).GetValue(i) ==
                                            (int)dbCollectionItemType.GetProperty(primaryKeyName).GetValue(dbEntity));

                                if (ent != null)
                                    context.UpdateDbEntity(dbEntity, ent);
                            });


                            var toBeAddedEntities = entityCollection.Where(e => (int)entityCollectionItemType.GetProperty(primaryKeyName).GetValue(e) == default(int)).ToList();
                            toBeAddedEntities.ForEach(e => context.Entry(e).State = EntityState.Added);
                        }
                    }
                }
            }
        }
    }
}