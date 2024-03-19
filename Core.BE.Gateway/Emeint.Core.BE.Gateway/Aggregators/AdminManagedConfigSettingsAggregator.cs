using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Gateway.Helpers.AggregatorsHelpers;
using Emeint.Core.BE.Gateway.Helpers.SerializationHelpers;
using MassTransit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Middleware.Multiplexer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Gateway.Aggregators
{
    public class AdminManagedConfigSettingsAggregator : IDefinedAggregator
    {
        private IConfiguration _configuration;
        private ILogger<AdminManagedConfigSettingsAggregator> _logger;
        public AdminManagedConfigSettingsAggregator(IConfiguration configuration, ILogger<AdminManagedConfigSettingsAggregator> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public Task<DownstreamResponse> Aggregate(List<DownstreamContext> responses)
        {
            var allSettings = new List<AdminConfigurationSettingHelper>();

            responses.ForEach(r =>
            {
                string downstreamReRoutekey = r.DownstreamReRoute.Key;
                string responseBody = r.DownstreamResponse?.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(responseBody))
                {
                    try
                    {
                        var parsedResponse = JsonConvert.DeserializeObject<Response<List<AdminConfigurationSettingHelper>>>(responseBody);

                        if (!downstreamReRoutekey.Contains("_")) //not payment settings result
                        {
                            downstreamReRoutekey = downstreamReRoutekey.Replace("SettingsResult", string.Empty);
                            downstreamReRoutekey = downstreamReRoutekey.Replace("AdminManaged", string.Empty);

                            parsedResponse.Data?.ForEach(d => { d.key = downstreamReRoutekey.ToLower() + "_" + d.key; });
                        }

                        if (parsedResponse.Data != null)
                            allSettings.AddRange(parsedResponse.Data);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Error while parsing response {ex.Message}");
                    }
                }

            });

            int expirationDurationInMins = Convert.ToInt32(_configuration["cache_response_expiration_mins"]);
            Response<List<AdminConfigurationSettingHelper>> response = new Response<List<AdminConfigurationSettingHelper>>();
            response.Data = allSettings;
            response.ErrorCode = 0;
            response.Expiration = new Expiration(expirationDurationInMins, 0, 0);

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(response, new JsonSerializerSettings() { ContractResolver = new SnakeCaseSerialzation().SerialzeResolver }), System.Text.Encoding.UTF8, "application/json");
            DownstreamResponse downstreamResponse = new DownstreamResponse(httpResponseMessage);
            return Task.FromResult(downstreamResponse);
        }
    }
}
