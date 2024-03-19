using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Repositories;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Concretes
{
    public class PermittedCountryService : IPermittedCountryService
    {
        private IPermittedCountryRepository _permittedCountryRepository;

        public PermittedCountryService(IPermittedCountryRepository permittedCountryRepository)
        {
            _permittedCountryRepository = permittedCountryRepository;
        }
        public List<PermittedCountry> GetPermittedCountries(string userId)
        {
            var PermittedCountries = _permittedCountryRepository.GetPermittedCountries(userId);
            return PermittedCountries;
        }
        public void AddPermittedCountryForUser(string userId, string countryCode,string createdBy)
        {
            _permittedCountryRepository.AddPermittedCountryForUser(userId, countryCode, createdBy);
        }
    }
}
