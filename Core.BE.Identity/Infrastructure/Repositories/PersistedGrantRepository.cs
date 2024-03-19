using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
using Emeint.Core.BE.Infrastructure.Repositories;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public class PersistedGrantRepository : IPersistedGrantRepository
    {

        private readonly PersistedGrantDbContext _context;
        public PersistedGrantRepository(PersistedGrantDbContext context)
        {
            _context = context;
        }

        public async Task RemoveExpiredPersistedGrantsAsync(string userId)
        {
            var grantsToRemove = _context.PersistedGrants.Where(g => g.SubjectId == userId && g.Expiration < DateTime.UtcNow);

            _context.PersistedGrants.RemoveRange(grantsToRemove);
            _context.SaveChanges();
        }
        public void RemoveAllPersistedGrants(string userId)
        {
            var grantsToRemove = _context.PersistedGrants.Where(g => g.SubjectId == userId);

            _context.PersistedGrants.RemoveRange(grantsToRemove);
            _context.SaveChanges();
        }
    }
}
