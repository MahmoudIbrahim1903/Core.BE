using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Emeint.Core.BE.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public class PermittedCountryRepository : BaseRepository<PermittedCountry, ApplicationDbContext>, IPermittedCountryRepository
    {
        public PermittedCountryRepository(ApplicationDbContext context) : base(context)
        {

        }

        public List<PermittedCountry> GetPermittedCountries(string userId)
        {
            return _context.PermittedCountries.Where(e => e.ApplicationUserId == userId).ToList();
        }

        public void AddPermittedCountryForUser(string userId, string countryCode, string createdBy)
        {
            var permittedCountry = new PermittedCountry
            {
                ApplicationUserId = userId,
                CountryCode = countryCode,
                CreatedBy = createdBy,
                CreationDate = DateTime.UtcNow
            };
            _context.PermittedCountries.Add(permittedCountry);
            _context.SaveChanges();
        }


    }
}
