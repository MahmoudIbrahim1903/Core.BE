using System;
using System.Runtime.Serialization;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion
{

    [DataContract]
    public class ApplicationVersionViewModel
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string CreationDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public string ModificationDate { get; set; }
        [DataMember]
        public bool IsBeta { get; set; }
        [DataMember]
        public bool IsDiscontinued { get; set; }
        [DataMember]
        public Domain.Enums.Platform Platform { get; set; }
        [DataMember]
        public string StoreURL { get; set; }

        public ApplicationVersionViewModel()
        {

        }

        public ApplicationVersionViewModel(Domain.Model.ApplicationVersion applicationVersion)
        {
            Code = applicationVersion.Code;
            Version = applicationVersion.Version;
            CreatedBy = applicationVersion.CreatedBy;
            CreationDate = applicationVersion.CreationDate != null ? applicationVersion.CreationDate.ToString("yyyyMMddmmHHss") : null;
            ModificationDate = applicationVersion.ModificationDate != null ? applicationVersion.ModificationDate.Value.ToString("yyyyMMddmmHHss") : null;
            ModifiedBy = applicationVersion.ModifiedBy;
            IsBeta = applicationVersion.IsBeta;
            IsDiscontinued = applicationVersion.IsDiscontinued;
            Platform = applicationVersion.Platform;
            StoreURL = applicationVersion.StoreUrl;
        }
    }
}
