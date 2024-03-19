using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Configurations.Controllers;
using Emeint.Core.BE.Configurations.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Emeint.Core.BE.SMS.Controllers.Admin
{
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiVersion("1.0")]
    [Route("api/sms/admin/configurations/v{version:apiVersion}")]
    public class AdminSmsConfigurationController : AdminConfigurationController
    {
        public AdminSmsConfigurationController
            (ISettingRepository settingRepository, IIdentityService identityService) 
            : base(settingRepository, identityService)
        {

        }
    }
}