using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public interface IMappingBehavior
    {
        void MapProperties<TSource, TDest>(TSource source, TDest dest);
    }
}
