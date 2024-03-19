using Emeint.Core.BE.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Emeint.Core.BE.Infrastructure.Repositories
{
    public class EnumerationRepository<TEnumeration, TContext> : BaseRepository<TEnumeration, TContext>, IEnumerationRepository<TEnumeration>
        where TEnumeration : Enumeration
        where TContext : DbContext
    {
        public EnumerationRepository(TContext context) : base(context)
        {
        }
    }
}
