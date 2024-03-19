using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.SMS.Application.ViewModels;
using Emeint.Core.BE.SMS.Domain.Configurations;
using Emeint.Core.BE.SMS.Domain.Enums;
using Emeint.Core.BE.SMS.Domain.Managers;
using Emeint.Core.BE.SMS.Domain.SmsProviders;
using Microsoft.AspNetCore.Mvc;

namespace Emeint.Core.BE.SMS.Controllers.Server
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/server/sms/v{version:apiVersion}")]
    public class ServerSmsController : Controller
    {
        private readonly ISmsMessageManager _smsMessageManager;
        private readonly IHashingManager _hashingManager;
        public ServerSmsController(ISmsMessageManager smsMessageManager,IHashingManager hashingManager)
        {
            _smsMessageManager = smsMessageManager;
            _hashingManager = hashingManager;
        }
        [Route("send")]
        [HttpPost]
        public async Task<BaseResponse> Send([FromBody] SmsViewModel sms)
        {
            
            #region Validate Hashed request
            string expectedSign = Request.Headers["Signature"];
            string sign = _hashingManager.HashSendSmsRequest(sms);
            if (string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(expectedSign) 
                || sign != expectedSign)
                throw new HashedRequestMismatchSignatureException();
            #endregion

            await _smsMessageManager.SendSmsAsync(sms);

            return new BaseResponse
            {
                ErrorCode = (int)ErrorCodes.Success,
                ErrorMsg = ErrorCodes.Success.ToString()
            };
        }
    }
}