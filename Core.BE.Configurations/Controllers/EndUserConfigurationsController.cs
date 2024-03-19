using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Configurations.Application.Mappers;
using Emeint.Core.BE.Configurations.Application.ViewModels;
using Emeint.Core.BE.Configurations.Domain.Enums;
using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Emeint.Core.BE.Configurations.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ConfigurationController : Controller
    {
        ISettingRepository _settingRepository;
        IIdentityService _identityService;
        public ConfigurationController(ISettingRepository settingRepository, IIdentityService identityService)
        {
            _settingRepository = settingRepository;
            _identityService = identityService;
        }

        [Route("settings")]
        [HttpGet]
        public async Task<Response<List<BaseSettingViewModel>>> GetEndUserSettings()
        {
            var settings = _settingRepository
                .GetSettings(null, null, SettingUser.EndUser);
            return new Response<List<BaseSettingViewModel>>()
            {
                Data = SettingMapper.MapFromSettingList(settings),
                ErrorCode = (int)ErrorCodes.Success
            };
        }

    }
}