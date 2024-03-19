using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class Mapper<TSource, TDest> : IMapper<TSource, TDest>
        where TDest : class, new() 
        where TSource : class 
    {
        protected readonly IList<Action<TSource, TDest>> Mappings = new List<Action<TSource, TDest>>();

        public virtual IMapper<TSource, TDest> AddMapping(Action<TSource, TDest> mapping)
        {
            Mappings.Add(mapping);
            return this;
        }

        protected virtual void CopyMatchingProperties(TSource source, TDest dest)
        {
            foreach (
                var destProp in
                    typeof (TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof (MappingIgnorAttribute), false).Any()))
            {
                var sourceProp = typeof (TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == destProp.Name && p.PropertyType == destProp.PropertyType);

                if (sourceProp != null && source != null)
                {
                    destProp.SetValue(dest, sourceProp.GetValue(source, null), null);
                }
            }
        }

        public virtual TDest MapObject(TSource source, TDest dest)
        {
            CopyMatchingProperties(source, dest);
            foreach (var action in Mappings)
            {
                action(source, dest);
            }

            return dest;
        }

        public virtual TDest CreateMappedObject(TSource source)
        {
            var dest = new TDest();
            return MapObject(source, dest);
        }

        public virtual List<TDest> CreateMappedObject(List<TSource> sources)
        {
            return sources.Select(CreateMappedObject).ToList();
        }
    
        //TODO: support maping of nested list, array, complex object ...
    }
}
