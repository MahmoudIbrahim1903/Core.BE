using Emeint.Core.BE.Identity.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Contracts
{
    public interface ICountryService
    {
        Country AddCountry(Country entity);
        void SaveEntities(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
        void UpdateCountry(Country entity);
        Task<Country> GetCountryAsync(int id, bool deepLoad = false);

        Country GetCountryByCode(string code);
        City GetCityByCode(string code);
        Area GetAreaByCode(string code);
        List<City> GetCities(bool deepLoad = false);
        List<Area> GetAreas();
        List<Country> GetCountries(bool deepLoad = false);
        List<Currency> GetCurrencies();
        //List<CountryRole> GetCountryRoles();
    }
}
