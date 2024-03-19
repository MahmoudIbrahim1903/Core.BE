using Emeint.Common.APIs.ViewModel;
using Emeint.Common.Infrastructure.Cache;
using Emeint.Common.Infrastructure.ObjectMapper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


/*
 * note: why reflection is used to call method in EntityManager while we have an instance of it !!!!!
 * because the generic method expects a type identifier, not a an instance of the Type class. 
 * please refere to the link below to find answer
 * http://stackoverflow.com/questions/1408120/how-to-call-generic-method-with-a-given-type-object
 */

namespace Emeint.Common.Infrastructure.GenericCRUDOperations
{
    public class CRUDOperationsManager
    {
        private static CRUDOperationsManager _instance;
        public static CRUDOperationsManager Instance
        {
            get
            {
                return _instance ?? (_instance = new CRUDOperationsManager());
            }
        }
        private CRUDOperationsManager()
        { }

        public IList Get(Type modelType, string model, Type viewModelType, EntityManager entityManager)
        {



            //MethodInfo nvalidateMe = typeof(EntityManager).GetMethod("Invalidate");
            //MethodInfo generic = method.MakeGenericMethod(targetType);
            //generic.Invoke(_entityManager, null);


            MethodInfo getMethod = typeof(EntityManager).GetMethod("Get");
            MethodInfo getGeneric = getMethod.MakeGenericMethod(modelType);
            var models = getGeneric.Invoke(entityManager, new object[] { null, true }) as IList;

            if (model == "ViewModel")
            {

                var mapperGeneric = typeof(DefaultRecursiveMapper<,>);
                var mapper = mapperGeneric.MakeGenericType(new Type[] { modelType, viewModelType }).GetConstructor(new Type[] { }).Invoke(new object[] { }); ;
                var mapperType = mapper.GetType();


                var mapMethod = mapperType.GetMethod("CreateMappedObject", new[] { models.GetType() });
                return mapMethod.Invoke(mapper, new object[] { models }) as IList;
            }
            return models;
        }
        public object GetObjectById(Type modelType, int key, EntityManager entityManager)
        {

            MethodInfo addMethod = typeof(EntityManager).GetMethod("GetObjectById");
            MethodInfo addGeneric = addMethod.MakeGenericMethod(modelType);
            var entity = addGeneric.Invoke(entityManager, new object[] { new object[] { key } });

            return entity;
        }

        public object GetById(Type modelType, int key, EntityManager entityManager)
        {

            MethodInfo addMethod = typeof(EntityManager).GetMethod("GetById");
            MethodInfo addGeneric = addMethod.MakeGenericMethod(modelType);
            var entity = addGeneric.Invoke(entityManager, new object[] { new object[] { key } });

            return entity;
        }


        public object Add(Type modelType, string obj, EntityManager entityManager)
        {

            var myObj = JsonConvert.DeserializeObject(obj, modelType);


            return RecursiveAdd(myObj, modelType, entityManager);
        }

        public object Update(Type modelType, string obj, EntityManager entityManager)
        {

            var myObj = JsonConvert.DeserializeObject(obj, modelType);

            //return UpdateObject(modelType, myObj, _entityManager);
            return RecursiveUpdate(myObj, modelType, entityManager);
        }

        public ModelMetadata GetTableMetadata(Type objType, EntityManager entityManager)
        {
            var modelType = objType;

            var addMethod = typeof(EntityManager).GetMethods().FirstOrDefault(m => m.Name == "GetTableMetadata");
            var addGeneric = addMethod.MakeGenericMethod(modelType);
            var entity = addGeneric.Invoke(entityManager, new object[] { });

            return entity as ModelMetadata;
        }
        private Object RecursiveAdd(Object myObj, Type objType, EntityManager entityManager)
        {

            //creating new temp object
            var tempObj = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(myObj), objType);

            //loop over all foreign objects & set them with null
            foreach (
                var myProp in
                objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any()))
            {
                var propValue = myProp.GetValue(tempObj);
                if (propValue == null) continue;
                var idProp = propValue.GetType().GetProperty("Id");
                if (idProp == null) continue;

                myProp.SetValue(tempObj, null);
            }

            //loop on all foreign lists and set them with null
            var listsOfEntities = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any() && p.PropertyType.Namespace == "System.Collections.Generic");

            foreach (var listType in listsOfEntities)
            {
                listType.SetValue(tempObj, null);
            }
            //adding the temp Object into DB
            //try
            //{
            var serverObject = AddObject(objType, tempObj, entityManager);
            //}
            //catch (Exception e)
            //{
            //}
            //setting the portal object Id with the added server objectId
            var serverId = serverObject.GetType().GetProperty("Id").GetValue(serverObject);
            serverObject.GetType().GetProperty("Id").SetValue(myObj, serverId);


            //Important Fix for Multitenant in case of using CrudOperationsManager : **** tarekn 06/08/2015
            //if (objType.BaseType == typeof(MultiTenant))
            //{
            //    serverObject.GetType().GetProperty("ServiceId").SetValue(myObj, Identity.Instance.ServiceId);

            //    objType.BaseType.GetProperty("ServiceId", typeof(int)).SetValue(myObj, Identity.Instance.ServiceId);
            //}

            foreach (var listType in listsOfEntities)
            {
                if (listType.GetValue(myObj) == null) continue;
                var idForeignProp = listType.PropertyType.GetGenericArguments()[0].GetProperty(objType.Name + "Id");

                if (idForeignProp == null) continue;

                var list = listType.GetValue(myObj) as IEnumerable;

                foreach (var portalItem in list)
                {
                    var idProp = portalItem.GetType().GetProperty(objType.Name + "Id");
                    if (idProp == null) continue;
                    idProp.SetValue(portalItem, serverId);
                }
            }

            //do recuresive update
            return RecursiveUpdate(myObj, objType, entityManager);
        }

        private Object RecursiveUpdate(Object myObj, Type objType, EntityManager entityManager)
        {
            int key = (int)myObj.GetType().GetProperty("Id").GetValue(myObj);
            var serverObj = GetObjectById(objType, key, entityManager);

            var contextObj = entityManager.UnitOfWork.GetType().GetProperty("DataContext").GetValue(entityManager.UnitOfWork);
            var context = contextObj as System.Data.Entity.DbContext;

            //update scalar properties
            UpdateObject(objType, myObj, entityManager);


            //loop on all foreign enities and add, edit, or update them
            //repeat recursively

            foreach (
               var myProp in
                   objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any()))
            {
                var propValue = myProp.GetValue(myObj);
                if (propValue == null) continue;
                var idProp = propValue.GetType().GetProperty("Id");
                if (idProp == null) continue;

                //check if property type has a db collection
                if (context != null)
                {
                    System.Data.Entity.DbSet dbSet = null;
                    try
                    {
                        dbSet = context.Set(myProp.PropertyType);
                    }
                    catch
                    {
                        // ignored
                    }

                    if (dbSet != null)
                    {
                        //check if object needs to be added or updated
                        int? nestedKey = idProp.GetValue(propValue) as int?;
                        var serverNestedObject = dbSet.Find(nestedKey);

                        if (serverNestedObject != null)
                        {
                            //update existing item, recursively
                            RecursiveUpdate(myProp.GetValue(myObj), myProp.PropertyType, entityManager);
                        }
                        else
                        {
                            //add new object
                            AddObject(myProp.GetType(), myProp.GetValue(myObj), entityManager);
                        }

                        //get edited item and insert it in the server object property value
                        var savedProp = dbSet.Find(nestedKey);
                        myProp.SetValue(serverObj, savedProp);

                        InvalidateType(myProp.GetType(), entityManager);
                    }
                }
            }


            //loop on all foreign lists and add, edit, or update them
            //repeat recursively
            var listsOfEntities = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any() && p.PropertyType.Namespace == "System.Collections.Generic");

            foreach (var listType in listsOfEntities)
            {
                //if generic items are db entities
                var itemType = listType.PropertyType.GenericTypeArguments[0];
                if (context != null)
                {
                    System.Data.Entity.DbSet dbSet = null;
                    try
                    {
                        dbSet = context.Set(itemType);
                    }
                    catch
                    {
                        // ignored
                    }

                    if (dbSet != null)
                    {
                        var list = listType.GetValue(myObj) as IEnumerable;
                        if (list == null) continue;



                        var newListType = typeof(List<>).MakeGenericType(itemType);
                        IList newList = Activator.CreateInstance(newListType) as IList;

                        //TODO: delete items removed from server list
                        //get server list and delete unmatched items
                        var toDelete = new List<Object>();


                        var serverListProp = serverObj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .FirstOrDefault(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any() && p.PropertyType.Namespace == "System.Collections.Generic" && p.PropertyType.GenericTypeArguments[0].Name == itemType.Name);

                        if (serverListProp == null) continue;
                        //var serverList = new Object() as IList;
                        var serverList = serverListProp.GetValue(serverObj) as IEnumerable;
                        if (serverList != null)
                        {


                            foreach (var serverItem in serverList)
                            {
                                bool matched = false;
                                var idProp = serverItem.GetType().GetProperty("Id");
                                if (idProp == null) break;
                                int serverId = (int)idProp.GetValue(serverItem);

                                foreach (var portalItem in list)
                                {
                                    int portalId = (int)idProp.GetValue(portalItem);
                                    if (portalId == serverId)
                                    {
                                        matched = true;
                                        break;
                                    }
                                }

                                if (!matched)
                                {
                                    toDelete.Add(serverItem);
                                }


                            }

                            //delete items in delete list
                            foreach (var deletedItem in toDelete)
                            {
                                try
                                {
                                    //TODO: handle if returns false
                                    DeleteObject(itemType, deletedItem, entityManager);
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }

                                try
                                {
                                    //serverList.Remove(deletedItem);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                            InvalidateType(itemType, entityManager);
                        }




                        foreach (var item in list)
                        {
                            //check if object needs to be added or updated

                            var idProp = item.GetType().GetProperty("Id");
                            if (idProp == null)
                            {
                                //var newItem = RecursiveAdd(item, item.GetType(),_entityManager);
                                //newList.Add(newItem);
                                continue;
                            }

                            int? nestedKey = idProp.GetValue(item) as int?;
                            var serverNestedObject = dbSet.Find(nestedKey);

                            if (serverNestedObject != null)
                            {
                                //update existing item, recursively
                                RecursiveUpdate(item, item.GetType(), entityManager);
                            }
                            else
                            {
                                //add new object
                                AddObject(item.GetType(), item, entityManager);
                            }

                            //get edited item and insert it in the server object property value
                            var savedProp = dbSet.Find(idProp.GetValue(item));
                            newList.Add(savedProp);
                        }
                        //set server list to new list
                        listType.SetValue(serverObj, newList);
                        InvalidateType(itemType, entityManager);


                    }

                }
            }

            InvalidateType(objType, entityManager);
            entityManager.UnitOfWork.SaveChanges();

            //get the full edited object and return it
            return GetObjectById(objType, key, entityManager);
        }
        public object AddObject(Type modelType, Object myObj, EntityManager entityManager)
        {


            MethodInfo addMethod = typeof(EntityManager).GetMethod("Add");
            MethodInfo addGeneric = addMethod.MakeGenericMethod(modelType);
            var entity = addGeneric.Invoke(entityManager, new[] { myObj });

            return entity;
        }
        public object UpdateObject(Type modelType, Object myObj, EntityManager entityManager)
        {


            int key = (int)myObj.GetType().GetProperty("Id").GetValue(myObj);



            MethodInfo updateMethod = typeof(EntityManager).GetMethod("Update");
            MethodInfo addGeneric = updateMethod.MakeGenericMethod(modelType);
            var entity = addGeneric.Invoke(entityManager, new[] { myObj, new object[] { key } });

            return entity;
        }


        public bool Delete(Type modelType, string strkey, EntityManager entityManager)
        {
            bool success;

            //var myObj = JsonConvert.DeserializeObject(obj, modelType); 
            int key = (int.Parse(strkey));//myObj.GetType().GetProperty("Id").GetValue(myObj); 

            MethodInfo addMethod = typeof(EntityManager).GetMethod("Delete", new[] { typeof(object[]) });
            MethodInfo addGeneric = addMethod.MakeGenericMethod(modelType);
            var result = addGeneric.Invoke(entityManager, new object[] { new object[] { key } });


            bool.TryParse(result.ToString(), out success);
            return success;
        }

        public bool DeleteObject(Type modelType, Object myObj, EntityManager entityManager)
        {
            try
            {
                bool success;

                int key = (int)myObj.GetType().GetProperty("Id").GetValue(myObj);

                var addMethod = typeof(EntityManager).GetMethod("Delete");
                var addGeneric = addMethod.MakeGenericMethod(modelType);
                var result = addGeneric.Invoke(entityManager, new object[] { new object[] { key } });

                bool.TryParse(result.ToString(), out success);
                return success;
            }
            catch //kadr fel 2alolo
            {
                return false;
            }
        }

        public void InvalidateType(Type modelType, EntityManager entityManager)
        {
            MethodInfo invalidateMethod = typeof(EntityManager).GetMethod("Invalidate");
            Emeint.Core.Managers.LogsManager.Info("after GetMethod");
           
            MethodInfo addGeneric = invalidateMethod.MakeGenericMethod(modelType);
            Emeint.Core.Managers.LogsManager.Info("invalidateMethod.MakeGenericMethod");
            
            addGeneric.Invoke(entityManager, new object[] { });
            Emeint.Core.Managers.LogsManager.Info("after addGeneric.Invoke");
        }
    }
}
