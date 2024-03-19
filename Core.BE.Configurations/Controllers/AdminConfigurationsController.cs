using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class AdminConfigurationController : Controller
    {
        ISettingRepository _settingRepository;
        IIdentityService _identityService;
        public AdminConfigurationController(ISettingRepository settingRepository, IIdentityService identityService)
        {
            _settingRepository = settingRepository;
            _identityService = identityService;
        }


        [Route("update_setting")]
        [HttpPut]
        [Authorize(Roles = "SuperAdmin, ConfigurationsFullController")]
        public async Task<Response<bool>> UpdateSetting([FromBody] AdminEditSettingRequestVm admin_edit_setting_request_vm)
        {
            if (string.IsNullOrWhiteSpace(admin_edit_setting_request_vm.Key))
                throw new Emeint.Core.BE.Domain.Exceptions.MissingParameterException("Key");

            bool updated = await _settingRepository.UpdateSettingAsync(admin_edit_setting_request_vm.Key, admin_edit_setting_request_vm.Value, _identityService.Email);
            return new Response<bool>
            {
                Data = updated,
                ErrorCode = (int)ErrorCodes.Success
            };
        }

        [Route("settings")]
        [HttpGet]
        [Authorize]
        public async Task<Response<List<AdminSettingViewModel>>> GetAdminSettings()
        {
            var settings = _settingRepository
             .GetSettings(null, null, SettingUser.Admin, null);
            var result = await SettingMapper.MapFromAdminSettingListAsync(settings);
            return new Response<List<AdminSettingViewModel>>()
            {
                Data = result,
                ErrorCode = (int)ErrorCodes.Success
            };
        }

        [Route("managed_settings")]
        [HttpGet]
        [Authorize(Roles = "SuperAdmin, ConfigurationsReader, ConfigurationsFullController")]

        public async Task<Response<List<AdminSettingViewModel>>> GetAdminManagedSettings()
        {
            var settings = _settingRepository
               .GetSettings(null, null, null, true);
            var result = await SettingMapper.MapFromAdminSettingListAsync(settings);
            return new Response<List<AdminSettingViewModel>>()
            {
                Data = result,
                ErrorCode = (int)ErrorCodes.Success
            };
        }

        [Route("refresh_cache")]
        [HttpGet]
        public async Task<BaseResponse> RefreshCache()
        {
            await _settingRepository.RefreshCacheAsync();
            return new BaseResponse()
            {
                ErrorCode = (int)ErrorCodes.Success
            };
        }


        [Route("set_display_name_to_settings")]
        [HttpGet]
        public async Task<BaseResponse> SetDisplayName()
        {
            await _settingRepository.RefreshCacheAsync();
            var settings = _settingRepository
                .GetSettings(null, null, null, null);  // get all keys

            foreach (var setting in settings)
            {

                StringBuilder DisplayName = new StringBuilder(setting.Key.Length * 2);
                DisplayName.Append(setting.Key[0]);
                for (int i = 1; i < setting.Key.Length; i++) //get display name
                {
                    if (char.IsUpper(setting.Key[i]))
                        if ((setting.Key[i - 1] != ' ' && !char.IsUpper(setting.Key[i - 1])) ||
                            (char.IsUpper(setting.Key[i - 1]) &&
                             i < setting.Key.Length - 1 && !char.IsUpper(setting.Key[i + 1])))
                            DisplayName.Append(' ');
                    DisplayName.Append(setting.Key[i]);
                }

                setting.DisplayName = DisplayName.ToString();
            }

           await _settingRepository.SaveEntitiesAsync();

            return new BaseResponse()
            {
                ErrorCode = (int)ErrorCodes.Success
            };
            //return await SettingMapper.MapFromAdminSettingListAsync(settings);
        }
    }
}