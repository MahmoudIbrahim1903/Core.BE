using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class ExactMappingBehavior:IMappingBehavior
    {
        public void MapProperties<TSource, TDest>(TSource source,TDest dest)
        {

            foreach (
                var destProp in
                    typeof(TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any()))
            {
                var sourceProp = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == destProp.Name && p.PropertyType == destProp.PropertyType);

                if (sourceProp != null && source != null)
                {
                    destProp.SetValue(dest, sourceProp.GetValue(source, null), null);
                }
            }
        }
    }
}
