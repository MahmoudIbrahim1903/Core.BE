using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public class CountryRepository : BaseRepository<Country, CountryContext>, ICountryRepository
    {
        public CountryRepository(CountryContext context) : base(context)
        {
        }
        public IQueryable<City> GetCities()
        {
            return _context.Set<City>();
        }

        public IQueryable<Area> GetAreas()
        {
            return _context.Set<Area>();
        }

        public override IQueryable<Country> GetAll()
        {
            return base.GetAll();
            //return base.GetAll().Include(c => c.Cities).ThenInclude(city => city.Areas).Include(c => c.Currency);
        }
        public Country GetCountryByCode(string code)
        {
            return _context.Set<Country>().Where(c => c.Code == code).FirstOrDefault();
            //.Include(c => c.CountryRoles)
        }
        public City GetCityByCode(string code)
        {
            return _context.Set<City>().Where(c => c.Code == code).FirstOrDefault();
        }
        public List<City> GetCitiesWithAreas()
        {
            return _context.Set<City>().Include(city => city.Areas).ToList();
        }

        public Area GetAreaByCode(string code)
        {
            return _context.Set<Area>().Where(c => c.Code == code).FirstOrDefault();
        }
        //public IQueryable<CountryRole> GetCountryRoles()
        //{
        //    return _context.Set<CountryRole>();
        //}
        public IQueryable<Currency> GetCurrencies()
        {
            return _context.Set<Currency>();
        }

    }
}