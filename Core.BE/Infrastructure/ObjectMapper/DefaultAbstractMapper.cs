using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{

    public class DefaultAbstractMapper<TSource, TDest> : AbstractMapper<TSource, TDest>
        where TDest : class, new()
        where TSource : class
    {
        public DefaultAbstractMapper()
        {
            MappingBehaviors.Add(new ExactMappingBehavior());
            MappingBehaviors.Add(new LocalizedMappingBehavior());
            MappingBehaviors.Add(new CollectionMappingBehavior());
        }
    }
}
