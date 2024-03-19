using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser
{
    public class EndUserSearchConversationCriteriaViewModel
    {
        public List<Status> Statuses { get; set; }
    }
}
