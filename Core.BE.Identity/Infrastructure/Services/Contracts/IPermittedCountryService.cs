using Emeint.Core.BE.Identity.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Contracts
{
    public interface IPermittedCountryService
    {
        List<PermittedCountry> GetPermittedCountries(string userId);
        void AddPermittedCountryForUser(string userId, string countryCode, string createdBy);
    }
}
