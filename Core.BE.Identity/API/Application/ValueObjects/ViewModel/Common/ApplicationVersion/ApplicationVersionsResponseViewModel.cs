using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion
{
    [DataContract]
    public class ApplicationVersionsResponseViewModel
    {
        [DataMember]
        public List<ApplicationVersionViewModel> ApplicationVersionsList { get; set; }
        [DataMember]
        public int Length { get; set; }

        public ApplicationVersionsResponseViewModel()
        {
            ApplicationVersionsList = new List<ApplicationVersionViewModel>();
            Length = 0;
        }
    }
}
