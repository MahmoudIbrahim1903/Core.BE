
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class ExactRecursiveMapperModelToDB<TSource, TDest> : RecursiveMapperModelToDB<TSource, TDest>
        where TDest : class, new()
        where TSource : class
    {
        public ExactRecursiveMapperModelToDB()
        {
            MappingBehaviors.Add(new ExactMappingBehavior());
        }
    }
}
