using Emeint.Core.BE.Configurations.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Configurations.Application.ViewModels
{
    [DataContract]
    public class AdminSettingViewModel : BaseSettingViewModel
    {
        [DataMember]
        public SettingUser User { set; get; }

        [DataMember]
        public List<EnumType> EnumTypeDetails { set; get; }

        [DataMember]
        public string DisplayName { set; get; }

        [DataMember]
        public string Description { set; get; }
        [DataMember]
        public string UnitOfMeasure { set; get; }
    }
}
