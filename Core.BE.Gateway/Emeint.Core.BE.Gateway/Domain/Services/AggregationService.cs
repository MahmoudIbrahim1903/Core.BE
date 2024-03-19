using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.API.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Emeint.Core.BE.Gateway.Domain.Contracts;

namespace Emeint.Core.BE.Gateway.Domain.Services
{
    public class AggregationService : IAggregationService
    {

        private readonly IIdentityService _identityService;
        private readonly ILogger<AggregationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationsProxy _configurationsProxy;

        public AggregationService(IConfigurationsProxy configurationsProxy, IIdentityService identityService, ILogger<AggregationService> logger, IConfiguration configuration)
        {
            _configurationsProxy = configurationsProxy;
            _identityService = identityService;
            _logger = logger;
            _configuration = configuration;
        }

        #region Configurations
        public async Task<Response<bool>> UpdateSettings(string key, string value)
        {
            return await _configurationsProxy.UpdateConfiguration(key, value);
        }
        #endregion 
    }
}
