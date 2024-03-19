using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    public class GetEndUserConversationsCriteriaViewModel
    {
        public PaginationViewModel Pagination { get; set; }
        public EndUserSearchConversationCriteriaViewModel Criteria { get; set; }
    }
}
