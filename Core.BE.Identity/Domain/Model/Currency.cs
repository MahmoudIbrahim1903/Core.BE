using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Model
{
    public class Currency :Entity
    {
        public Currency() : base() {

            CreatedBy = "System";
        }  
        public Currency(int id, string name, string code) : this()
        {
            this.Id = id;
            Code = code;
            CurrencyCode = code;
            CurrencyNameEn = name;
        }

        public string CurrencyCode { get; set; }
        public string CurrencyNameAr { set; get; }
        public string CurrencyNameEn { set; get; }
    }
}
