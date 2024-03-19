using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class DefaultRecursiveMapper<TSource, TDest> : RecursiveMapper<TSource, TDest>
        where TDest : class, new()
        where TSource : class
    {
        public DefaultRecursiveMapper()
        {
            MappingBehaviors.Add(new ExactMappingBehavior());
            MappingBehaviors.Add(new LocalizedMappingBehavior());
            MappingBehaviors.Add(new CollectionMappingBehavior());
            MappingBehaviors.Add(new ImageMappingBehavior());
        }
    }
}
