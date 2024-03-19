using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin
{
    public class AdminCriteriaViewModel
    {
        public List<Status> Statuses { get; set; }
        public List<string> Types { get; set; }
        public string ToUserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
