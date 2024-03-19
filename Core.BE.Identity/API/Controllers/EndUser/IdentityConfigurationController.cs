using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Configurations.Controllers;
using Emeint.Core.BE.Configurations.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers.EndUser
{
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiVersion("1.0")]
    [Route("api/identity/configurations/v{version:apiVersion}")]
    public class IdentityConfigurationController : ConfigurationController
    {
        public IdentityConfigurationController(ISettingRepository settingRepository, IIdentityService identityService) : base(settingRepository, identityService)
        {

        }
    }
}