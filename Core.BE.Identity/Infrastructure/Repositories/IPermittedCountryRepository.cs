using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Domain.SeedWork;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public interface IPermittedCountryRepository: IRepository<PermittedCountry>
    {
        List<PermittedCountry> GetPermittedCountries(string userId);
        void AddPermittedCountryForUser(string userId, string countryCode, string createdBy);

    }
}
