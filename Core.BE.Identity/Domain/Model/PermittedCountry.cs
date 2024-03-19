using Emeint.Core.BE.Domain.SeedWork;
using System.Collections.Generic;

namespace Emeint.Core.BE.Identity.Domain.Model
{
    public class PermittedCountry: Entity
    {
        public string CountryCode { get; set; }
        //public Country Country { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

}
}
