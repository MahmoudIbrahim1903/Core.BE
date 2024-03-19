using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Gateway.Api.Application.ViewModels;
using Emeint.Core.BE.Gateway.Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Gateway.Api.Controllers.Admin
{
    [ApiVersion("1.0")]
    [Route("api/admin/aggregate/configurations/v{version:apiVersion}")]
    public class AdminConfigurationsController : ControllerBase
    {
        private readonly IAggregationService _aggregationService;
        public AdminConfigurationsController(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [Route("update_setting")]
        [HttpPut]
        [Authorize(Roles = "SuperAdmin, ConfigurationsFullController")]
        public async Task<Response<bool>> UpdateSetting([FromBody] AdminEditSettingRequestVm admin_edit_setting_request_vm)
        {
            if (string.IsNullOrWhiteSpace(admin_edit_setting_request_vm.Key))
                throw new Emeint.Core.BE.Domain.Exceptions.MissingParameterException("Key");

            return await _aggregationService.UpdateSettings(admin_edit_setting_request_vm.Key, admin_edit_setting_request_vm.Value);
        }
    }
}
