
using System.Collections.Generic;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    public class LoginResponse : BaseConnectResponse
    {
        public string user_id { set; get; }
        public string user_display_name { set; get; }
        public int error_code { get; set; }
        public string error_msg { get; set; }
        public string scope { get; set; }
        public List<string> allowed_roles { set; get; }
        public string phone_number { set; get; }
        public string email { set; get; }
    }
}