using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.Identity.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Repositories;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Concretes
{
    public class CountryService : ICountryService
    {
        private const string CACHE_REGION = "Country_Service";
        private readonly string ALL_COUNTRIES = "All_Countries";
        private readonly string ALL_CITIES = "All_Cities";
        private readonly string ALL_AREAS = "All_Areas";
        private readonly string ALL_CURRENCIES = "All_Currencies";
        private readonly string ALL_COUNTRIES_ROLES = "All_Country_Roles";

        private List<string> _itemsToBeClearedFromCacheRegion;

        private ICountryRepository _countryRepository;
        private ICacheService _cacheService;
        public CountryService(ICacheService cacheService, ICountryRepository countryRepository)
        {
            _cacheService = cacheService;
            _countryRepository = countryRepository;
            _itemsToBeClearedFromCacheRegion = new List<string>();
        }

        public List<Country> GetCountries(bool deepLoad = false)
        {
            var cachedCountries = (List<Country>)_cacheService.GetItemFromCacheRegion<List<Country>>(CACHE_REGION, ALL_COUNTRIES);

            if (cachedCountries == null)
            {
                cachedCountries = _countryRepository.GetAll().ToList();
                _cacheService.SaveItemToCacheRegion(CACHE_REGION, ALL_COUNTRIES, cachedCountries);
            }

            if (deepLoad)
            {
                var cities = GetCities();
                var areas = GetAreas();
                var currencies = GetCurrencies();
                //var countriesRoles = GetCountryRoles();

                return cachedCountries.Select(c =>
                {

                    var countryCurrency = currencies.FirstOrDefault(curr => curr.Id == c.CurrencyId);
                    var countryCities = cities.Where(city => city.CountryCode == c.Code).ToList();
                    countryCities.ForEach(city =>
                    {
                        city.Areas = areas.Where(a => a.CityCode == city.Code).ToList();
                    });

                    return new Country()
                    {
                        AllowMultipleRoles = c.AllowMultipleRoles,
                        //CountryRoles = countriesRoles.Where(cr => cr.CountryCode == c.Code).ToList(),
                        Cities = countryCities,
                        Code = c.Code,
                        Comment = c.Comment,
                        CreatedBy = c.CreatedBy,
                        CreationDate = c.CreationDate,
                        Currency = countryCurrency,
                        CurrencyId = c.CurrencyId,
                        DefaultLanguage = c.DefaultLanguage,
                        Id = c.Id,
                        InternationalDialCode = c.InternationalDialCode,
                        IsoCode = c.IsoCode,
                        ModificationDate = c.ModificationDate,
                        ModifiedBy = c.ModifiedBy,
                        NameAr = c.NameAr,
                        NameEn = c.NameEn,
                        PhoneNumberLength = c.PhoneNumberLength
                    };

                }).ToList();
            }

            return cachedCountries;
        }
        public List<City> GetCities(bool deepLoad = false)
        {
            var cachedCities = (List<City>)_cacheService.GetItemFromCacheRegion<List<City>>(CACHE_REGION, ALL_CITIES);
            if (cachedCities != null)
                return cachedCities;

            var cities = _countryRepository.GetCities().ToList();
            _cacheService.SaveItemToCacheRegion(CACHE_REGION, ALL_CITIES, cities);

            if (deepLoad)
            {
                var areas = GetAreas();

                return cities.Select(c => new City()
                {
                    Areas = areas.Where(a => a.CityCode == c.Code).ToList(),
                    Code = c.Code,
                    Comment = c.Comment,
                    CountryCode = c.CountryCode,
                    CreatedBy = c.CreatedBy,
                    CreationDate = c.CreationDate,
                    Id = c.Id,
                    IsActiveOperations = c.IsActiveOperations,
                    ModifiedBy = c.ModifiedBy,
                    ModificationDate = c.ModificationDate,
                    NameAr = c.NameAr,
                    NameEn = c.NameEn
                }).ToList();
            }

            return cities;
        }
        public List<Area> GetAreas()
        {
            var cachedAreas = (List<Area>)_cacheService.GetItemFromCacheRegion<List<Area>>(CACHE_REGION, ALL_AREAS);
            if (cachedAreas != null)
                return cachedAreas;

            var areas = _countryRepository.GetAreas().ToList();
            _cacheService.SaveItemToCacheRegion(CACHE_REGION, ALL_AREAS, areas);
            return areas;
        }
        //public List<CountryRole> GetCountryRoles()
        //{
        //    var cachedRoles = (List<CountryRole>)_cacheService.GetItemFromCacheRegion<List<CountryRole>>(CACHE_REGION, ALL_COUNTRIES_ROLES);
        //    if (cachedRoles != null)
        //        return cachedRoles;

        //    var roles = _countryRepository.GetCountryRoles().ToList();
        //    _cacheService.SaveItemToCacheRegion(CACHE_REGION, ALL_COUNTRIES_ROLES, roles);
        //    return roles;
        //}
        public List<Currency> GetCurrencies()
        {
            var cachedCurrencies = (List<Currency>)_cacheService.GetItemFromCacheRegion<List<Currency>>(CACHE_REGION, ALL_CURRENCIES);
            if (cachedCurrencies != null)
                return cachedCurrencies;

            var currencies = _countryRepository.GetCurrencies().ToList();
            _cacheService.SaveItemToCacheRegion(CACHE_REGION, ALL_CURRENCIES, currencies);
            return currencies;
        }

        public virtual Country AddCountry(Country entity)
        {
            if (_itemsToBeClearedFromCacheRegion.Contains(ALL_COUNTRIES) == false)
                _itemsToBeClearedFromCacheRegion.Add(ALL_COUNTRIES);

            return _countryRepository.Add(entity);
        }
        public virtual void UpdateCountry(Country entity)
        {
            if (_itemsToBeClearedFromCacheRegion.Contains(ALL_COUNTRIES) == false)
                _itemsToBeClearedFromCacheRegion.Add(ALL_COUNTRIES);

            _countryRepository.Update(entity);
        }
        public void SaveEntities(CancellationToken cancellationToken = default(CancellationToken))
        {
            _countryRepository.SaveEntities(cancellationToken);
            ClearRequiredCacheItems();
        }
        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = _countryRepository.SaveEntitiesAsync(cancellationToken);
            ClearRequiredCacheItems();
            return result;
        }

        public Area GetAreaByCode(string code)
        {
            var all = GetAreas();
            var item = all.FirstOrDefault(i => i.Code == code);
            return item;
        }
        public Task<Country> GetCountryAsync(int id, bool deepLoad = false)
        {
            var all = GetCountries(deepLoad);
            var item = all.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(item);
        }
        public City GetCityByCode(string code)
        {
            var all = GetCities(false);
            var item = all.FirstOrDefault(i => i.Code == code);
            return item;
        }
        public Country GetCountryByCode(string code)
        {
            var countries = GetCountries(false);
            //var countriesRoles = GetCountryRoles();

            var country = countries.FirstOrDefault(c => c.Code == code);
            if (country == null)
                throw new CountryNotFoundException(code);

            return new Country()
            {
                AllowMultipleRoles = country.AllowMultipleRoles,
                Code = country.Code,
                Comment = country.Comment,
                //CountryRoles = countriesRoles.Where(cr => cr.CountryCode == code).ToList(),
                CreatedBy = country.CreatedBy,
                CreationDate = country.CreationDate,
                CurrencyId = country.CurrencyId,
                DefaultLanguage = country.DefaultLanguage,
                Id = country.Id,
                InternationalDialCode = country.InternationalDialCode,
                IsoCode = country.IsoCode,
                ModificationDate = country.ModificationDate,
                ModifiedBy = country.ModifiedBy,
                NameAr = country.NameAr,
                NameEn = country.NameEn,
                PhoneNumberLength = country.PhoneNumberLength
            };
        }


        private void ClearRequiredCacheItems()
        {
            _itemsToBeClearedFromCacheRegion.ForEach(item =>
            {

                _cacheService.DeleteItemFromCacheRegion(CACHE_REGION, item);
            });
            _itemsToBeClearedFromCacheRegion.Clear();
        }

    }
}
