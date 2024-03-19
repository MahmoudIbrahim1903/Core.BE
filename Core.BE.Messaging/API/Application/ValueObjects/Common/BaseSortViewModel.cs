using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    public class BaseSortViewModel
    {
        public SortDirection? SortDirection { get; set; }
    }
}
