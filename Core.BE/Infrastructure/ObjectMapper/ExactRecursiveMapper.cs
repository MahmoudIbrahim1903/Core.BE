using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class ExactRecursiveMapper<TSource, TDest> : RecursiveMapper<TSource, TDest>
        where TDest : class, new()
        where TSource : class
    {
        public ExactRecursiveMapper()
        {
            MappingBehaviors.Add(new ExactMappingBehavior());
        }
    }
}
