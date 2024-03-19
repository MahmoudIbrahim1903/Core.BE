using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Mailing.Domain.Configurations;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.API.Filters
{
    public class WhiteListFilter : ActionFilterAttribute
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly ILogger _logger;

        public WhiteListFilter(IConfigurationManager configurationManager, ILogger<WhiteListFilter> logger)
        {
            _configurationManager = configurationManager;
            _logger = logger;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_configurationManager.EnableWhiteList())
            {
                var clientIP = context.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
                _logger.LogInformation($"client ip {clientIP}  method name: {context?.HttpContext?.Request?.Path.ToString() ?? string.Empty}");


                var isInWhiteList = _configurationManager.GetWhiteList().Split(",").ToList()
                    .Any(i => i.Trim().Equals(clientIP));

                if (!isInWhiteList)
                    throw new UnauthorizedActionException();
            }

        }
    }
}
