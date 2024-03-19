using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    public class CriteriaViewModel
    {
        //public DateTime? From { get; set; }
        //public DateTime? To { get; set; }
        public List<Status> Statuses { get; set; }
        public List<string> Types { get; set; }
        public Direction Direction { get; set; }
        public int? CurrentMessageId { get; set; }
        public string ToUserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime SentDateFrom { get; set; }
        public List<MessageSource> NotificationSources { get; set; }
    }
}
