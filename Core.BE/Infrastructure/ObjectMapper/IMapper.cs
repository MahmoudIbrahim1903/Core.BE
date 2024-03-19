using System;
using System.Collections.Generic;

namespace Emeint.Common.Infrastructure.ObjectMapper
{
    public interface IMapper<TSource, TDest>
    {
        IMapper<TSource, TDest> AddMapping(Action<TSource, TDest> mapping);
        TDest CreateMappedObject(TSource source);
        List<TDest> CreateMappedObject(List<TSource> sources);
    }
}