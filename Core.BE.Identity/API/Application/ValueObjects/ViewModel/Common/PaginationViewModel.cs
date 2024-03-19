using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common
{
    [DataContract]
    public class PaginationViewModel
    {
        [DataMember]
        public int? PageSize { get; set; }
        [DataMember]
        public int? PageNumber { get; set; }
    }
}
