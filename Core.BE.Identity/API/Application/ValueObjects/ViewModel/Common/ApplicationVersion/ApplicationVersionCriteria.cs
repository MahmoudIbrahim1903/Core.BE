using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion
{
    public class ApplicationVersionCriteria : BaseSearchCriteria
    {
        public string Version { get; set; }
        public bool? IsBeta { get; set; }
        public bool? IsDiscontinued { get; set; }
        public Domain.Enums.Platform? Platform { get; set; }
        public VersionSortingViewModel Sorting { get; set; }
        //public bool? ForceUpdated { get; set; }
    }
}
