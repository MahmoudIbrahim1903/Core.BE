using System.Collections.Generic;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Admin
{
    public class AdminVM : BaseAdminVM
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
        public string CreationDate { get; set; }
        public string LastLoginDate { get; set; }
    }
}