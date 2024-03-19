using Emeint.Core.BE.Domain.SeedWork;
using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public interface IPersistedGrantRepository
    {
        Task RemoveExpiredPersistedGrantsAsync(string userId);
        void RemoveAllPersistedGrants(string userId);

    }
}
