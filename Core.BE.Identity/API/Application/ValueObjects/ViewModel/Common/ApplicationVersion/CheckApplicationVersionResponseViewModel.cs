using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion
{
    public class CheckApplicationVersionResponseViewModel
    {
        [DataMember]
        public bool IsLatest { get; set; }
        [DataMember]
        public bool ForceUpdate { get; set; }
        [DataMember]
        public string StoreUrl { get; set; }

    }
}
