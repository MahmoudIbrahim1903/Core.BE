using System.ComponentModel.DataAnnotations;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    public class LoginViewModel
    {
        public string username { get; set; }

        public string password { get; set; }

        public string client_id { get; set; }

        public string client_secret { get; set; }

        public string grant_type { get; set; }
    }
}