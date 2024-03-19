using System.Collections.Generic;

namespace Emeint.Core.BE.Domain.SeedWork
{
    public interface IEnumerationManager<TEnumeration>
    {
        List<TEnumeration> Get();
        TEnumeration Get(string code);

    }
}
