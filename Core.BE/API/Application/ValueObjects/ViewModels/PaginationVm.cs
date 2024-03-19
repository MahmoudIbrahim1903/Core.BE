using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Emeint.Core.BE.API.Application.ValueObjects.ViewModels
{
    [DataContract]
    public class PaginationVm
    {
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public int PageNumber { get; set; }
    }
}
