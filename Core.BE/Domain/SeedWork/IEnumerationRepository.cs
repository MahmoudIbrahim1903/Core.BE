using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.Domain.SeedWork
{
    public interface IEnumerationRepository<TEnumerator> : IRepository<TEnumerator>
        where TEnumerator : Enumeration
    {
    }
}
