using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common
{
    public class ResetPasswordWithOTPRequestVm
    {
        public string MobileNumber { get; set; }
        public string Otp { get; set; }
        public string NewPassword { set; get; }
    }
}
