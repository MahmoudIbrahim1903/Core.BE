using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class CollectionMappingBehavior:IMappingBehavior
    {
        public void MapProperties<TSource, TDest>(TSource source, TDest dest)
        {
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
                    .FirstOrDefault(p => p.Name == destProp.Name && p.PropertyType.Namespace == GenericCollectionsNamespace );

                if (sourceProp == null) return;

                //Check if the types of the generics are the same or can be mapped as ViewModels
                var SourceGenericTypeNames= new List<String>();
                for (int i = 0; i < sourceProp.PropertyType.GenericTypeArguments.Length; i++)
                {
                    SourceGenericTypeNames.Add(sourceProp.PropertyType.GenericTypeArguments[i].Name);
                }

                var DestGenericTypeNames = new List<String>();
                for (int i = 0; i < destProp.PropertyType.GenericTypeArguments.Length; i++)
                {
                    DestGenericTypeNames.Add(destProp.PropertyType.GenericTypeArguments[i].Name);
                }

                if(DestGenericTypeNames.Count==SourceGenericTypeNames.Count)
                {
                    Boolean SimilarTypes=true;
                    for (int i = 0; i < DestGenericTypeNames.Count; i++)
                    {
                        if (DestGenericTypeNames[i]!=SourceGenericTypeNames[i])
                        {
                            SimilarTypes = false;
                        }
                    }

                    //Boolean MappableTypes = true;
                    //for (int i = 0; i < DestGenericTypeNames.Count; i++)
                    //{
                    //    if (DestGenericTypeNames[i] != SourceGenericTypeNames[i])
                    //    {
                    //        MappableTypes = false;
                    //    }
                    //}


                    
                    if(SimilarTypes)
                    //Copy the Collection directly
                    {
                        if (sourceProp != null && source != null)
                        {
                            destProp.SetValue(dest, sourceProp.GetValue(source, null), null);
                        }
                    }

                }

                //debug
                //var s = sourceProp.GetValue(source, null);
                //var d = destProp.GetValue(dest, null);


//Set the equal properties
                //if (sourceProp != null && source != null)
                //{
                //    destProp.SetValue(dest, sourceProp.GetValue(source, null), null);
                //}
            }
        }
    }
}
