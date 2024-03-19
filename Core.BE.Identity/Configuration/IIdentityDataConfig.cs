using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Configuration
{
    public interface IIdentityDataConfig
    {
        IEnumerable<Client> GetClients();
        IEnumerable<IdentityResource> GetResources();
        IEnumerable<ApiResource> GetApis();
    }
}
