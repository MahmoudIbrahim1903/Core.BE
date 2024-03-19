using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Gateway.Api.Application.ViewModels;
using Emeint.Core.BE.Gateway.Domain.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Gateway.HttpAgrregators.Proxies
{
    public class ConfigurationsProxy : IConfigurationsProxy
    {
        private readonly IWebRequestUtility _webRequestUtility;
        private readonly IConfiguration _configuration;

        public ConfigurationsProxy(IWebRequestUtility webRequestUtility, IConfiguration configuration)
        {
            _webRequestUtility = webRequestUtility;
            _configuration = configuration;
        }

        #region Configurations
        public async Task<Response<bool>> UpdateConfiguration(string key, string value)
        {
            string prefix = key.Split('_')[0];
            key = key.Replace($"{prefix}_", "");

            string settingsUrls = _configuration["UpdateSettingsUrls"]; //"prefix1=url; prefix2=url"
            var settingsUrlsList = settingsUrls.Split(';').ToList();
            var settingPrefixUrl = settingsUrlsList.FirstOrDefault(s => s.Trim().StartsWith($"{prefix}="));

            if (settingPrefixUrl == null || settingPrefixUrl.Split('=').Length < 2)
                throw new InternalServerErrorException($"Update setting url not found for {prefix}!");

            AdminEditSettingRequestVm adminEditSettingRequestVm = new AdminEditSettingRequestVm() { Key = key, Value = value };
            var uri = settingPrefixUrl.Split('=')[1].Trim();
            var httpRequest = new Emeint.Core.BE.Domain.SeedWork.HttpPutRequest(uri, null, new List<KeyValuePair<string, string>>(), adminEditSettingRequestVm, "application/json", JsonNamingStrategy.SnakeCase);
            _webRequestUtility.AddCurrentRequestHeaders(httpRequest);
            var result = await _webRequestUtility.PutExtended<bool>(httpRequest, JsonNamingStrategy.SnakeCase);
            return result;
        }
        #endregion
    }
}
