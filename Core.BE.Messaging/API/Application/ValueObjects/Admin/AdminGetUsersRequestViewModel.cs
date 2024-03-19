using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin
{
    public class AdminGetUsersRequestViewModel
    {
        public PaginationViewModel Pagination { get; set; }
    }
}
