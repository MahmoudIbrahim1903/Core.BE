using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Configurations.Controllers;
using Emeint.Core.BE.Configurations.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Emeint.Core.BE.Chatting.Controllers.Admin
{
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiVersion("1.0")]
    [Route("api/admin/chatting/configurations/v{version:apiVersion}")]
    public class ChattingAdminConfigurationController : AdminConfigurationController
    {
        public ChattingAdminConfigurationController(ISettingRepository settingRepository, IIdentityService identityService) : base(settingRepository, identityService)
        {

        }
    }
}
