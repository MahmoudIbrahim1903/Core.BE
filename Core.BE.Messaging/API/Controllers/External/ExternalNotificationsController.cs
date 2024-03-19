using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.External;
using Emeint.Core.BE.Notifications.Domain.Enums;
using Emeint.Core.BE.Notifications.Domain.Manager;
using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Emeint.Core.BE.Notifications.API.Controllers.Server
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/notifications/v{version:apiVersion}")]
    public class ExternalNotificationsController : Controller
    {
        #region Fields
        private readonly string _typeName;
        private readonly ILogger<ExternalNotificationsController> _logger;
        private readonly IHashingManager _hashingManager;
        private readonly IMessageAggregateService _messageAggregateService;
        #endregion

        #region Constructor
        public ExternalNotificationsController(ILogger<ExternalNotificationsController> logger, IHashingManager hashingManager, IMessageAggregateService messageAggregateService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _typeName = GetType().Name;
            _hashingManager = hashingManager;
            _messageAggregateService = messageAggregateService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Send Notification from server event
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Send Notification Failed</response>
        [Route("send")]
        [HttpPost]
        public async Task<BaseResponse> Send([FromBody] ExternalMessageViewModel notification)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            if (notification == null)
                throw new MissingParameterException("notification");

            #region Validate Hashed request
            string expectedSign = Request.Headers["Signature"];
            var sign = _hashingManager.HashSendMessageRequest(notification);
            if (string.IsNullOrEmpty(sign) || string.IsNullOrEmpty(expectedSign) || sign != expectedSign)
                throw new HashedRequestMismatchSignatureException();
            #endregion

            _logger.LogInformation($"{_typeName}.{methodName}: Saving send notification in database, from: " + "System Message");

            // Save request in database
            SendNotificationRequestDto messageFull = new SendNotificationRequestDto();
            messageFull.MessageTypeCode = notification.MessageTypeCode;
            messageFull.Destinations = new List<MessageDestinationsViewModel>()
                {
                    new MessageDestinationsViewModel()
                    {
                            DestinationType = notification.Destinations.DestinationType,
                            Recipients = notification.Destinations.Recipients,
                    }
                };
            messageFull.DisplayParams = new List<string> { notification.Body };
            messageFull.DisplayParamsAr = new List<string> { notification.Body };
            messageFull.DisplayParamsSw = new List<string> { notification.Body };
            messageFull.TitleParams = new List<string> { notification.Title };
            messageFull.TitleParamsAr = new List<string> { notification.Title };
            messageFull.TitleParamsSw = new List<string> { notification.Title };
            messageFull.MessageSource = MessageSource.SystemEventMessage;
            messageFull.Alert = true;

            var result = await _messageAggregateService.Send(messageFull);

            return new BaseResponse() { ErrorCode = (int)ErrorCodes.Success };
        }
        #endregion

    }
}