using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.SeedWork;

namespace Emeint.Core.BE.Identity.Domain.Model
{
    public class Country : Entity, IAggregateRoot
    {
        public Country()
        {
            Cities = new HashSet<City>();
            //TenantCountries = new HashSet<TenantCountry>();
        }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int? CurrencyId { set; get; }
        public Currency Currency { set; get; }
        public string IsoCode { get; set; }
        public string InternationalDialCode { get; set; }
        public int PhoneNumberLength { get; set; }
        public string DefaultLanguage { get; set; }
        public bool AllowMultipleRoles { set; get; }
        public bool? IsEnabled { set; get; }
        public virtual ICollection<City> Cities { get; set; }
        //public virtual ICollection<TenantCountry> TenantCountries { get; set; }
        //public virtual ICollection<CountryRole> CountryRoles { set; get; }
    }
}
