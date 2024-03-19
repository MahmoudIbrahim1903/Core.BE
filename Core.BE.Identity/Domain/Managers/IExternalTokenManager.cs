using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Managers
{
    public interface IExternalTokenManager
    {
        Task<string> GetExternalProviderToken(string userName);
    }
}
