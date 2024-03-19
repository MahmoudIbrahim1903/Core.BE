using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Admin
{
    public class UpdateAdminVM : BaseAdminVM
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }

    }
}
