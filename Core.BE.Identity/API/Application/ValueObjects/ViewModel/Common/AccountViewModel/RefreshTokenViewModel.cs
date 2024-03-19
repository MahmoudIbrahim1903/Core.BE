
namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    public class RefreshTokenViewModel
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string refresh_token { set; get; }
    }
}
