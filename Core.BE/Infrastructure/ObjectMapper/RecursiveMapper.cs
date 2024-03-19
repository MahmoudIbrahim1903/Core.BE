using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class RecursiveMapper<TSource, TDest> : AbstractMapper<TSource, TDest>
        where TDest : class, new()
        where TSource : class
    {
        protected override void CopyMatchingProperties(TSource source, TDest dest)
        {
            //map 1st level properties
            foreach (IMappingBehavior Behavior in MappingBehaviors)
            {
                Behavior.MapProperties<TSource, TDest>(source, dest);
            }

            //find nested properties
            foreach (
                var destProp in
                    typeof(TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any()))
            {
                //get all props in source that have the type name + "ViewModel" = destination type name
                //TODO optimize this refelection
                var sourceProp = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.PropertyType.Name + "ViewModel" == destProp.PropertyType.Name);

                //TODO handle this with a predicate

                if (sourceProp != null && source != null)
                {
                    //type of source nested prop
                    Type TNestedSource = sourceProp.PropertyType;
                    //type of dest nested prop
                    Type TNestedDest = destProp.PropertyType;

                    // Create a new mapper based on nested source and dest types
                    var NestedRecursiveMapper = typeof(RecursiveMapper<,>).MakeGenericType(new Type[] { TNestedSource, TNestedDest }).GetConstructor(new Type[] { }).Invoke(new object[] { });

                    //copy the current mapping behaviors to the new mapper 
                    NestedRecursiveMapper.GetType().GetProperty("MappingBehaviors").SetValue(NestedRecursiveMapper, MappingBehaviors);

                    //get the source nested object
                    var sourceValue = sourceProp.GetValue(source, null);

                    //run mapper on source nested object
                    var NestedMappedProperty = NestedRecursiveMapper.GetType().GetMethod("CreateMappedObject", new Type[] { TNestedSource }).Invoke(NestedRecursiveMapper, new object[] { sourceValue });

                    //set the dest property to the mapped source prop
                    destProp.SetValue(dest, NestedMappedProperty, null);
                }

            }

            //Map nested collections of ViewModels
            String GenericCollectionsNamespace = "System.Collections.Generic";
            foreach (
                //Get generic collection props from the dest
               var destProp in
                   typeof(TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any() && p.PropertyType.Namespace == GenericCollectionsNamespace))
            {
                var DestGenericTypes = destProp.PropertyType.GenericTypeArguments;
                //Get generic collection props from source
                var sourceProp = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == destProp.Name && p.PropertyType.Namespace == GenericCollectionsNamespace);

                if (sourceProp == null) return;

                var SourceGenericTypes = sourceProp.PropertyType.GenericTypeArguments;
                //Check if the types of the generics are the same or can be mapped as ViewModels
                var SourceGenericTypeNames = new List<String>();
                for (int i = 0; i < sourceProp.PropertyType.GenericTypeArguments.Length; i++)
                {
                    SourceGenericTypeNames.Add(sourceProp.PropertyType.GenericTypeArguments[i].Name);
                }

                var DestGenericTypeNames = new List<String>();
                for (int i = 0; i < destProp.PropertyType.GenericTypeArguments.Length; i++)
                {
                    DestGenericTypeNames.Add(destProp.PropertyType.GenericTypeArguments[i].Name);
                }

                if (DestGenericTypeNames.Count == 1 && SourceGenericTypeNames.Count == 1)
                //for 1 generic type only
                {

                    Boolean MappableTypes = true;
                    for (int i = 0; i < DestGenericTypeNames.Count; i++)
                    {
                        if (DestGenericTypeNames[i] != SourceGenericTypeNames[i] + "ViewModel")
                        {
                            MappableTypes = false;
                        }
                    }



                    if (MappableTypes)
                    //Copy the Collection directly
                    {
                        if (sourceProp != null && source != null)
                        {
                            //generic type of source collection
                            Type TNestedSource = SourceGenericTypes[0];
                            //generic type of dest collection
                            Type TNestedDest = DestGenericTypes[0];

                            // Create a new mapper based on nested source and dest types
                            var NestedRecursiveMapper = typeof(RecursiveMapper<,>).MakeGenericType(new Type[] { TNestedSource, TNestedDest }).GetConstructor(new Type[] { }).Invoke(new object[] { });

                            //copy the current mapping behaviors to the new mapper 
                            NestedRecursiveMapper.GetType().GetProperty("MappingBehaviors").SetValue(NestedRecursiveMapper, MappingBehaviors);

                            //get the source collection items
                            // casting to ICollection fails
                            var sourceCollection = sourceProp.GetValue(source, null) as IEnumerable;
                            //var SourceItems = Array.CreateInstance(TNestedDest, 10);
                            //sourceCollection.CopyTo(SourceItems, 0);
                            //var MappedItems = new List<Object>();

                            //list to set in dest
                            var DestList = typeof(List<>).MakeGenericType(new Type[] { TNestedDest }).GetConstructor(new Type[] { }).Invoke(new object[] { }) as IList;

                            
                            foreach (var item in sourceCollection)
                            {
                                var mappedItem = NestedRecursiveMapper.GetType().GetMethod("CreateMappedObject", new Type[] { TNestedSource }).Invoke(NestedRecursiveMapper, new object[] { item });
                                DestList.Add(mappedItem);
                            }
                            destProp.SetValue(dest, DestList, null);
                            
                            ////run mapper on source items
                            //for (int i = 0; i < SourceItems.Length; i++)
                            //{
                            //    var mappedItem = NestedRecursiveMapper.GetType().GetMethod("CreateMappedObject", new Type[] { TNestedSource }).Invoke(NestedRecursiveMapper, new object[] { SourceItems.GetValue(1) });
                            //    destCollection.Add(mappedItem);
                            //    //SourceItems.SetValue(mappedItem, i);
                            //}


                        }
                    }

                }
            }


        }

    }
}
