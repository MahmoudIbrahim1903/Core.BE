using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Configurations.Controllers;
using Emeint.Core.BE.Configurations.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Emeint.Core.BE.Chatting.Controllers.EndUser
{

    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiVersion("1.0")]
    [Route("api/chatting/configurations/v{version:apiVersion}")]
    public class ChattingConfigurationController: ConfigurationController
    {
        public ChattingConfigurationController(ISettingRepository settingRepository, IIdentityService identityService) : base(settingRepository, identityService)
        {

        }
    }
}
