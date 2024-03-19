using Emeint.Core.BE.Identity.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.SeedWork;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public interface ICountryRepository : IRepository<Country>
    {
        Country GetCountryByCode(string code);
        City GetCityByCode(string code);
        Area GetAreaByCode(string code);
        List<City> GetCitiesWithAreas();
        IQueryable<City> GetCities();
        IQueryable<Area> GetAreas();
        //IQueryable<CountryRole> GetCountryRoles();
        IQueryable<Currency> GetCurrencies();

    }
}