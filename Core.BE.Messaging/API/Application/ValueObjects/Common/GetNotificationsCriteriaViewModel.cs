using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    public class GetNotificationsCriteriaViewModel
    {
        public PaginationViewModel Pagination { get; set; }
        public SortingViewModel Sorting { get; set; }
        public CriteriaViewModel Criteria { get; set; }
    }
}
