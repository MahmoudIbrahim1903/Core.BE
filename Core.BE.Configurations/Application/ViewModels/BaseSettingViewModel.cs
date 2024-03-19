using Emeint.Core.BE.Configurations.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Configurations.Application.ViewModels
{
    [DataContract]
    public class BaseSettingViewModel
    {
        [DataMember]
        public string Key { set; get; }
        [DataMember]
        public string Value { set; get; }

        [DataMember]
        public bool IsRequired { set; get; }
        [DataMember]
        public string Group { set; get; }
        [DataMember]
        public SettingType SettingType { set; get; }
    }
}
