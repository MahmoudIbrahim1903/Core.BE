
namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    public class RefreshTokenResponse : BaseConnectResponse
    {
        public string id_token { set; get; }
        public string scope { set; get; }
    }
}
