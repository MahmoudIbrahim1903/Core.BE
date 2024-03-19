using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    class DateTimeMappingBehavior : IMappingBehavior
    {
        public void MapProperties<TSource, TDest>(TSource source, TDest dest)
        {
            // we thought that keeping the date time object as a time stamp in SQL server would be a better idea than using the datetime api object and then converting to .Net Datetime

        }
    }
}

