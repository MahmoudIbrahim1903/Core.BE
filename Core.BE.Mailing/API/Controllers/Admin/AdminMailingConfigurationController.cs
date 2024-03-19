using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Configurations.Controllers;
using Emeint.Core.BE.Configurations.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Emeint.Core.BE.Mailing.API.Controllers.Admin
{
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiVersion("1.0")]
    [Route("api/mailing/admin/configurations/v{version:apiVersion}")]
    public class AdminMailingConfigurationController : AdminConfigurationController
    {
        public AdminMailingConfigurationController(ISettingRepository settingRepository, IIdentityService identityService) : base(settingRepository, identityService)
        {

        }
    }
}