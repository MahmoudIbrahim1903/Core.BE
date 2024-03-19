using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels
{
    public class VideoViewModel
    {
        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
