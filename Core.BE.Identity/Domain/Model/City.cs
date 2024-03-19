using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Emeint.Core.BE.Domain.SeedWork;

namespace Emeint.Core.BE.Identity.Domain.Model
{
    public class City : Entity
    {
        public City()
        {
            Areas = new HashSet<Area>();
        }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool IsActiveOperations { get; set; }
        public virtual ICollection<Area> Areas {  get; set; }

        [ForeignKey("Country")]
        public string CountryCode { get; set; }

        [Required]
        public Country Country { get; set; }
    }
}