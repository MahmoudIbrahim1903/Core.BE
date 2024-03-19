using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    public class PaginationViewModel
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
    }
}
