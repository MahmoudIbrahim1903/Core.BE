using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.SeedWork;

namespace Emeint.Core.BE.Identity.Domain.Model
{
    public class Area : Entity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        [ForeignKey("City")]
        public string CityCode { get; set; }
        
        [Required]
        public City City { get; set; }
    }
}