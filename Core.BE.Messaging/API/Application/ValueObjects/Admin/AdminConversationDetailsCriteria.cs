﻿using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin
{
    public class AdminConversationDetailsCriteria
    {
        public List<Status> Statuses { get; set; }
        public List<string> Types { get; set; }
        public string FirstPeerId { get; set; }
        public string SecondPeerId { get; set; }
    }
}
