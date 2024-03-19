using System;
using System.Collections.Generic;
using System.Text;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    public class LogoutRequestVm
    {
        public string ClientId { set; get; }
        public string ClientSecret { set; get; }
        public string RefreshToken { set; get; }
    }
}
