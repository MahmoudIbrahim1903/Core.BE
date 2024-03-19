using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion
{
    public class ActionOnApplicationVersionViewModel
    {
        public string Version { get; set; }
        public Domain.Enums.Platform Platform { get; set; }
        public bool IsDiscontinued { get; set; }
        public bool IsBeta { get; set; }
        public string StoreUrl { get; set; }
        //public bool ForceUpdated { get; set; }
        public OperationOnApplicationVersion OperationOnApplicationVersion { get; set; }
    }
}
