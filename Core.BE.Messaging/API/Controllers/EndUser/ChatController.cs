//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using Emeint.Core.BE.API.Infrastructure.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
//using Emeint.Core.BE.Domain.Exceptions;
//using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
//using Emeint.Core.BE.Domain.Enums;
//using Emeint.Core.BE.Notifications.Domain.Constants;
//using Emeint.Core.BE.Notifications.Domain.Enums;
//using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
//using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;

//namespace Emeint.Core.BE.Notifications.API.Controllers.EndUser
//{
//    [Produces("application/json")]
//    [Route("api/messaging/chat")]
//    public class ChatController : Controller
//    {
//        #region Fields
//        private readonly IIdentityService _identityService;
//        private readonly ILogger<MessagingController> _logger;
//        private readonly string _typeName;
//        private readonly IMessageAggregateService _messageAggregateService;

//        #endregion

//        #region Constructor
//        public ChatController(ILogger<MessagingController> logger, IIdentityService identityService,
//                              IMessageAggregateService messageAggregateService)
//        {
//            _identityService = identityService;
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _typeName = GetType().Name;
//            _messageAggregateService = messageAggregateService;


//        }
//        #endregion

//        #region Methods

//        /// <summary>
//        /// Send Chat message
//        /// </summary>
//        /// <response code="200 (error_code=1)">Internal Server Error</response>
//        [Route("send")]
//        [HttpPost]
//        [Authorize]
//        public async Task<BaseResponse> Send([FromBody]EndUserMessageRequestViewModel message, [FromHeader(Name = "Request-Client-Reference-Number")] string requestClientReferenceNumber)//, [FromHeader(Name = "Language")] string language)
//        {
//            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

//            if (message == null)
//                throw new MissingParameterException("message");
//            //string userName = user.FirstName + " " + user.LastName;
//            _logger.LogInformation($"{_typeName}.{methodName}: Send message BE Messaging Send, from: " + message.ToUserId);

//            //var user = _identityService.UserId;

//            //type is chat
//            MessageViewModel messageFull = new MessageViewModel();
//            messageFull.DisplayParams = new List<string>();
//            messageFull.DisplayParams.Add(message.Body);
//            messageFull.MessageDestinations.Add(new MessageDestinationsViewModel()
//            {
//                DestinationType = DestinationType.User,
//                Recipients = new List<string>() { message.ToUserId }
//            });
//            messageFull.MessageTypeCode = MessageTypes.Text;
//            //messageFull.TitleEn = MessageTypes.Chat_Message;
//            messageFull.MessageSource = MessageSource.EndUserMessage;
//            messageFull.SenderId = _identityService.UserId;
//            messageFull.Alert = message.Alert;

//            var result = await _messageAggregateService.Send(messageFull);

//            return new Response<bool>() { ErrorCode = (int)ErrorCodes.Success };
//        }

//        /// <summary>
//        /// Get Received and Sent message
//        /// </summary>
//        /// <response code="200 (error_code=1)">Internal Server Error</response>
//        /// <response code="200 (error_code=110)">Get Messages Failed</response>
//        [Route("get_conversation_details")]
//        [HttpPost]
//        [Authorize]
//        public async Task<Response<List<EndUserConversationMessageDetailsResponseModel>>> GetConversationDetails([FromBody]GetEndUserConversationDetailsCriteriaViewModel criteria)
//        {
//            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

//            _logger.LogInformation($"{_typeName}.{methodName}: Getting End User Messages from repository");

//            var result = _messageAggregateService.GetEndUserConversationDetailsByCriteria(criteria, _identityService.UserId);

//            return new Response<List<EndUserConversationMessageDetailsResponseModel>>() { Data = result };
//        }


//        /// <summary>
//        /// Get conversations 
//        /// </summary>
//        /// <response code="200 (error_code=1)">Internal Server Error</response>
//        /// <response code="200 (error_code=110)">Get conversations Failed</response>
//        [Route("get_conversations")]
//        [HttpPost]
//        [Authorize]
//        public async Task<Response<List<EndUserConversationsResponseModel>>> GetConversations([FromBody]GetEndUserConversationsCriteriaViewModel criteria)
//        {
//            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
//            _logger.LogInformation($"{_typeName}.{methodName}: Getting End User conversations from repository");

//            var result = _messageAggregateService.GetEndUserConversationsByCriteria(criteria, _identityService.UserId);
//            return new Response<List<EndUserConversationsResponseModel>>() { Data = result };
//        }
//        #endregion
//    }
//}//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using Emeint.Core.BE.API.Infrastructure.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
//using Emeint.Core.BE.Domain.Exceptions;
//using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
//using Emeint.Core.BE.Domain.Enums;
//using Emeint.Core.BE.Notifications.Domain.Constants;
//using Emeint.Core.BE.Notifications.Domain.Enums;
//using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
//using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;

//namespace Emeint.Core.BE.Notifications.API.Controllers.EndUser
//{
//    [Produces("application/json")]
//    [Route("api/messaging/chat")]
//    public class ChatController : Controller
//    {
//        #region Fields
//        private readonly IIdentityService _identityService;
//        private readonly ILogger<MessagingController> _logger;
//        private readonly string _typeName;
//        private readonly IMessageAggregateService _messageAggregateService;

//        #endregion

//        #region Constructor
//        public ChatController(ILogger<MessagingController> logger, IIdentityService identityService,
//                              IMessageAggregateService messageAggregateService)
//        {
//            _identityService = identityService;
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _typeName = GetType().Name;
//            _messageAggregateService = messageAggregateService;


//        }
//        #endregion

//        #region Methods

//        /// <summary>
//        /// Send Chat message
//        /// </summary>
//        /// <response code="200 (error_code=1)">Internal Server Error</response>
//        [Route("send")]
//        [HttpPost]
//        [Authorize]
//        public async Task<BaseResponse> Send([FromBody]EndUserMessageRequestViewModel message, [FromHeader(Name = "Request-Client-Reference-Number")] string requestClientReferenceNumber)//, [FromHeader(Name = "Language")] string language)
//        {
//            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

//            if (message == null)
//                throw new MissingParameterException("message");
//            //string userName = user.FirstName + " " + user.LastName;
//            _logger.LogInformation($"{_typeName}.{methodName}: Send message BE Messaging Send, from: " + message.ToUserId);

//            //var user = _identityService.UserId;

//            //type is chat
//            MessageViewModel messageFull = new MessageViewModel();
//            messageFull.DisplayParams = new List<string>();
//            messageFull.DisplayParams.Add(message.Body);
//            messageFull.MessageDestinations.Add(new MessageDestinationsViewModel()
//            {
//                DestinationType = DestinationType.User,
//                Recipients = new List<string>() { message.ToUserId }
//            });
//            messageFull.MessageTypeCode = MessageTypes.Text;
//            //messageFull.TitleEn = MessageTypes.Chat_Message;
//            messageFull.MessageSource = MessageSource.EndUserMessage;
//            messageFull.SenderId = _identityService.UserId;
//            messageFull.Alert = message.Alert;

//            var result = await _messageAggregateService.Send(messageFull);

//            return new Response<bool>() { ErrorCode = (int)ErrorCodes.Success };
//        }

//        /// <summary>
//        /// Get Received and Sent message
//        /// </summary>
//        /// <response code="200 (error_code=1)">Internal Server Error</response>
//        /// <response code="200 (error_code=110)">Get Messages Failed</response>
//        [Route("get_conversation_details")]
//        [HttpPost]
//        [Authorize]
//        public async Task<Response<List<EndUserConversationMessageDetailsResponseModel>>> GetConversationDetails([FromBody]GetEndUserConversationDetailsCriteriaViewModel criteria)
//        {
//            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

//            _logger.LogInformation($"{_typeName}.{methodName}: Getting End User Messages from repository");

//            var result = _messageAggregateService.GetEndUserConversationDetailsByCriteria(criteria, _identityService.UserId);

//            return new Response<List<EndUserConversationMessageDetailsResponseModel>>() { Data = result };
//        }


//        /// <summary>
//        /// Get conversations 
//        /// </summary>
//        /// <response code="200 (error_code=1)">Internal Server Error</response>
//        /// <response code="200 (error_code=110)">Get conversations Failed</response>
//        [Route("get_conversations")]
//        [HttpPost]
//        [Authorize]
//        public async Task<Response<List<EndUserConversationsResponseModel>>> GetConversations([FromBody]GetEndUserConversationsCriteriaViewModel criteria)
//        {
//            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
//            _logger.LogInformation($"{_typeName}.{methodName}: Getting End User conversations from repository");

//            var result = _messageAggregateService.GetEndUserConversationsByCriteria(criteria, _identityService.UserId);
//            return new Response<List<EndUserConversationsResponseModel>>() { Data = result };
//        }
//        #endregion
//    }
//}