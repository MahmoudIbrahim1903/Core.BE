using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class AbstractMapper<TSource, TDest> : Mapper<TSource, TDest>
        // Copied from Taha, what does this do?
        where TDest : class, new()
        where TSource : class
    {
        public List<IMappingBehavior> MappingBehaviors { get; set; }


        public AbstractMapper()
        {
            MappingBehaviors = new List<IMappingBehavior>();
        }

        /// <summary>
        /// Maps the properties of source to destination according to the Mapping behaviors (Strategy Pattern)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// 
        protected override void CopyMatchingProperties(TSource source, TDest dest)
        {
            foreach (IMappingBehavior Behavior in MappingBehaviors)
            {
                Behavior.MapProperties<TSource, TDest>(source, dest);
            }

            //foreach (
            //    var destProp in
            //        typeof(TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //            .Where(p => p.CanWrite && !p.GetCustomAttributes(typeof(MappingIgnorAttribute), false).Any()))
            //{
            //    var sourceProp = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //        .FirstOrDefault(p => p.Name == destProp.Name && p.PropertyType == destProp.PropertyType);

            //    if (sourceProp != null && source != null)
            //    {
            //        destProp.SetValue(dest, sourceProp.GetValue(source, null), null);
            //    }
            //}
        }
    }
}
