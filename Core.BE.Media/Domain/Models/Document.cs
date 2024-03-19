using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Models
{
    public class Document : Entity
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public bool IsDeleted { set; get; }
        public string DeletedBy { set; get; }
        public DateTime? DeletedDate { set; get; }
    }
}
