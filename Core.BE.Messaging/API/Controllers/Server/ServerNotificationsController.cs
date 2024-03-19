using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;
using Emeint.Core.BE.Notifications.Domain.Enums;
using Emeint.Core.BE.Notifications.Domain.Manager;
using Emeint.Core.BE.Notifications.Services.Serivces.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Emeint.Core.BE.Notifications.API.Controllers.Server
{
    [Produces("application/json")]
    [Route("api/server/notifications/v{version:apiVersion}")]
    [ApiVersion("1.0")]
    public class ServerNotificationsController : Controller
    {
        #region Fields
        private readonly ILogger<ServerNotificationsController> _logger;
        private readonly string _typeName;
        private readonly IHashingManager _hashingManager;
        private readonly IServerMessageService _serverMessageService;

        #endregion

        #region Constructor
        public ServerNotificationsController(ILogger<ServerNotificationsController> logger, IHashingManager hashingManager,
            IServerMessageService serverMessageService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _typeName = GetType().Name;
            _hashingManager = hashingManager;
            _serverMessageService = serverMessageService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Send notification from server event
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Send Notification Failed</response>
        [Route("send")]
        [HttpPost]
        public async Task<BaseResponse> Send([FromBody]SystemEventNotificationDto notification)
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
                            Recipients = notification.Destinations.FirstOrDefault().Recipients,
                            DestinationType = DestinationType.User
                    }
                };
            messageFull.DisplayParams = notification.DisplayParams;
            messageFull.DisplayParamsAr = notification.DisplayParamsAr;
            messageFull.DisplayParamsSw = notification.DisplayParamsSw;
            messageFull.TitleParams = notification.TitleParams;
            messageFull.TitleParamsAr = notification.TitleParamsAr;
            messageFull.TitleParamsSw = notification.TitleParamsSw;
            messageFull.MessageSource = MessageSource.SystemEventMessage;
            messageFull.ExtraParams = notification.ExtraParams;
            messageFull.Alert = notification.Alert;
            messageFull.SendAsPush = notification.SendAsPush;

            var result = await _serverMessageService.Send(messageFull);
            return new BaseResponse() { ErrorCode = (int)ErrorCodes.Success };
        }

        #endregion

    }
}