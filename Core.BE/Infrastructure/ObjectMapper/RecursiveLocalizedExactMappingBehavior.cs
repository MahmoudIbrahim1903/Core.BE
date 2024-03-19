using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public class RecursiveLocalizedExactMappingBehavior : IMappingBehavior
    {
        public void MapProperties<TSource, TDest>(TSource source, TDest dest)
        {
            //match nested view models
            StackTrace stackTrace = new StackTrace();

            //apply recursive mapping for each matched pair with the default options (Recursive, Localized, Exact)

            // Get calling method name
            var f =stackTrace.GetFrame(1).GetMethod();
            throw new NotImplementedException();
        }
    }
}
