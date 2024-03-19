using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Enums;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Notifications.Services.Serivces.Contracts;
using Emeint.Core.BE.Notifications.Domain.Configurations;
using Emeint.Core.BE.Domain.Exceptions;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.Notifications.API.Controllers.EndUser
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/notifications/v{version:apiVersion}")]
    public class NotificationsController : Controller
    {
        #region Fields
        private readonly IIdentityService _identityService;
        private readonly ILogger<NotificationsController> _logger;
        private readonly string _typeName;
        private readonly IEndUserMessageService _endUserMessageService;
        private readonly IConfigurationManager _configurationManager;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor
        public NotificationsController(ILogger<NotificationsController> logger, IIdentityService identityService,
                IEndUserMessageService endUserMessageService, IConfigurationManager configurationManager, IConfiguration configuration)
        {
            _identityService = identityService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _typeName = GetType().Name;
            _endUserMessageService = endUserMessageService;
            _configurationManager = configurationManager;
            _configuration = configuration;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Update notifications status for driver or owner - DEPRECATED
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=402)">Update Notification Status Failed</response>
        [Route("update_notifications_status")]
        [HttpPut]
        [Authorize]
        public async Task<BaseResponse> UpdateNotificationsStatusV1([FromBody] List<UpdateMessageStatusViewModel> notifications_status)
        {
            if (notifications_status == null)
                throw new MissingParameterException("notifications_with_status");


            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting statusCode from repository");

            _endUserMessageService.UpdateMessagesStatus(notifications_status, _identityService.UserId);

            return new BaseResponse() { ErrorCode = (int)ErrorCodes.Success };
        }

        /// <summary>
        /// Update notifications status for driver or owner
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=402)">Update Notification Status Failed</response>
        [Route("notifications_status")]
        [HttpPut]
        [Authorize]
        [ApiVersion("2.0")]
        public async Task<BaseResponse> UpdateNotificationsStatusV2([FromBody] List<UpdateMessageStatusViewModelV2> notifications_status)
        {
            if (notifications_status == null)
                throw new MissingParameterException("notifications_with_status");


            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting statusCode from repository");

            _endUserMessageService.UpdateMessagesStatus(notifications_status, _identityService.UserId);

            return new BaseResponse() { ErrorCode = (int)ErrorCodes.Success };
        }
        /// <summary>
        /// Update notifications status to read
        /// </summary>
        [Route("mark_all_as_read")]
        [HttpPut]
        [Authorize]
        public async Task<BaseResponse> MarkAllAsRead()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Mark all messages as read");

            _endUserMessageService.MarkAllMessagesAsRead(_identityService.UserId);

            return new BaseResponse() { ErrorCode = (int)ErrorCodes.Success };
        }

        /// <summary>
        /// Get Received notifications for end user - DEPRECATED
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Get Notifications Failed</response>
        [Route("notifications")]
        [HttpGet]
        [Authorize]
        public async Task<Response<List<EndUserNotificationResponseModel>>> GetNotificationsV1(SortBy? sort_by, SortDirection? sort_direction,
            int? page_number, int? page_size, Direction? direction, int? current_message_id, [FromHeader(Name = "Language")] Language language)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting End User Notification from repository");

            // TO DO: move VM preparations to aggregate service if needed
            GetNotificationsCriteriaViewModel getNotificationsCriteriaViewModel = new GetNotificationsCriteriaViewModel
            {
                Pagination = new PaginationViewModel { PageSize = page_size, PageNumber = page_number },

                Sorting = new SortingViewModel { SortBy = sort_by, SortDirection = sort_direction },

                Criteria = new CriteriaViewModel
                {
                    CurrentMessageId = current_message_id,
                    Direction = direction ?? Direction.next,
                    ToUserId = _identityService.UserId,
                    NotificationSources = new List<MessageSource> { MessageSource.SystemEventMessage }
                }
            };

            if (_configurationManager.GetTrackAdminNotifications())
                getNotificationsCriteriaViewModel.Criteria.NotificationSources.Add(MessageSource.AdminMessage);

            var result = _endUserMessageService.GetMessagesForEndUser(getNotificationsCriteriaViewModel, language);

            return new Response<List<EndUserNotificationResponseModel>>()
            {
                Data = result?.List
            };
        }


        /// <summary>
        /// Get Received notifications for end user
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Get Notifications Failed</response>
        [Route("notifications")]
        [HttpGet]
        [Authorize]
        [ApiVersion("2.0")]
        public async Task<Response<PagedList<EndUserNotificationResponseModelV2>>> GetNotificationsV2(SortBy? sort_by, SortDirection? sort_direction,
            int? page_number, int? page_size, Direction? direction, int? current_notification_id, [FromHeader(Name = "Language")] Language language)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting End User Notification from repository");

            var notificationTrackingSLAInDays = _configurationManager.GetNotificationsTrackingSLA();

            // TO DO: move VM preparations to aggregate service if needed
            GetNotificationsCriteriaViewModel getNotificationsCriteriaViewModel = new GetNotificationsCriteriaViewModel
            {
                Pagination = (page_number == null || page_size == null || page_number == 0 || page_size == 0) ? null : new PaginationViewModel { PageSize = page_size, PageNumber = page_number },

                Sorting = new SortingViewModel { SortBy = sort_by, SortDirection = sort_direction },

                Criteria = new CriteriaViewModel
                {
                    CurrentMessageId = current_notification_id,
                    Direction = direction ?? Direction.next,
                    ToUserId = _identityService.UserId,
                    SentDateFrom = DateTime.UtcNow.AddDays(-notificationTrackingSLAInDays),
                    NotificationSources = new List<MessageSource> { MessageSource.SystemEventMessage }
                }
            };

            if (_configurationManager.GetTrackAdminNotifications())
                getNotificationsCriteriaViewModel.Criteria.NotificationSources.Add(MessageSource.AdminMessage);

            var result = _endUserMessageService.GetMessagesForEndUser(getNotificationsCriteriaViewModel, language);

            return new Response<PagedList<EndUserNotificationResponseModelV2>>()
            {
                Data = result?.List == null ? null : new PagedList<EndUserNotificationResponseModelV2>
                {
                    TotalCount = result.TotalCount,
                    HasMore = result.HasMore,
                    List = result.List.Select(rs => new EndUserNotificationResponseModelV2
                    {
                        TypeCode = rs.MessageTypeCode,
                        Body = rs.Body,
                        // Code = rs.Code,
                        ExtraParams = rs.ExtraParams,
                        Id = rs.Id,
                        SentDate = rs.SentDate,
                        Title = rs.Title,
                    }).ToList()
                }
            };
        }
        /// <summary>
        /// Get Received notifications with count
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=10)">Invalid Parameter</response>
        //[Route("notifications_with_count")]
        //[HttpGet]
        //[Authorize]
        //[ProducesResponseType(typeof(Task<Response<GetNotificationsResponse>>), 200)]
        //public async Task<Response<GetNotificationsResponse>> GetNotificationsWithCount(SortBy sort_by, SortDirection sort_direction,
        //    int? page_number, int? page_size, Direction? direction, int? current_message_id, [FromHeader(Name = "Language")] string language)
        //{

        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

        //    _logger.LogInformation($"{_typeName}.{methodName}: Getting End User Notification from repository");


        //    // TO DO: move VM preparations to aggregate service if needed
        //    GetNotificationsCriteriaViewModel getNotificationsCriteriaViewModel = new GetNotificationsCriteriaViewModel();

        //    getNotificationsCriteriaViewModel.Pagination = new PaginationViewModel { PageSize = page_size, PageNumber = page_number };

        //    getNotificationsCriteriaViewModel.Sorting = new SortingViewModel { SortBy = sort_by, SortDirection = sort_direction };

        //    getNotificationsCriteriaViewModel.Criteria = new CriteriaViewModel
        //    {
        //        CurrentMessageId = current_message_id,
        //        Direction = direction ?? Direction.next,
        //        ToUserId = _identityService.UserId,
        //        MessageSources = new List<MessageSource> { MessageSource.AdminMessage,MessageSource.SystemEventMessage}
        //    };

        //    var result = _endUserMessageService.GetMessagesByCriteriaWithCount(getNotificationsCriteriaViewModel, language);

        //    _logger.LogInformation($"{_typeName}.{methodName}: Mapping model to view model");

        //    return new Response<GetNotificationsResponse>()
        //    {
        //        Data = result
        //    };

        //}

        /// <summary>
        /// Get Count of UnRead notifications  - DEPRECATED
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Get Notifications Count Failed</response>
        [Route("get_unread_notifications_count")]
        [HttpGet]
        [Authorize]
        public async Task<Response<int>> GetUnreadNotificationsCountV1()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting End User Unread Notification Count from repository");

            var result = _endUserMessageService.GetEndUserUnreadMessagesCount();

            return new Response<int>() { Data = result };
        }

        /// <summary>
        /// Get Count of UnRead notifications 
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Get Notifications Count Failed</response>
        [Route("unread_notifications_count")]
        [HttpGet]
        [Authorize]
        [ApiVersion("2.0")]
        public async Task<Response<int>> GetUnreadNotificationsCountV2()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting End User Unread Notification Count from repository");

            var result = _endUserMessageService.GetEndUserUnreadMessagesCount();

            return new Response<int>() { Data = result };
        }

        /// <summary>
        /// register user with his device platform // TODO handle anonymous users, so null in user id is not handled
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=400)">Register User Failed</response>
        /// <response code="200 (error_code=401)">Invalid User Id</response>
        [Route("register_for_push")]
        [HttpPost]
        [Authorize]
        public async Task<BaseResponse> RegisterForPush([FromBody] RegisterForPushViewModel register_for_push, [FromHeader(Name = "Platform")] string platform, [FromHeader(Name = "Language")] string language)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            if (register_for_push == null)
                throw new MissingParameterException("user");

            if (string.IsNullOrEmpty(platform))
                throw new MissingParameterException("Platform");

            if (register_for_push.UserId == null)
                throw new MissingParameterException("user_id");

            if (string.IsNullOrEmpty(register_for_push.PushNotificationToken))
                throw new MissingParameterException("push_notification_token");

            _logger.LogInformation($"{_typeName}.{methodName}: Register User BE Messaging Send, from: " + register_for_push.UserId);


            var result = await _endUserMessageService.RegisterForPush(register_for_push.UserId, platform, register_for_push.PushNotificationToken,
                register_for_push.DeviceId, language);


            return new Response<bool>() { Data = result.Data };
        }

        /// <summary>
        /// Unregister form Push Notification provider
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Send Message Failed</response>
        [Route("unregister_from_push_provider")]
        [Authorize]
        [HttpPost]
        public async Task<BaseResponse> UnRegisterFromPushProvider()
        {
            //To DO Implement Hashing
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var res = await _endUserMessageService.UnRegisterFromPush(_identityService.UserId);
            return new BaseResponse { ErrorCode = (int)ErrorCodes.Success };
        }

        /// <summary>
        ///  delete notification
        /// </summary>
        /// <param name="notification_id"></param>
        /// <returns></returns>
        [Route("{notification_id}")]
        [HttpDelete]
        [Authorize]
        public async Task<BaseResponse> DeleteNotification([FromRoute] int notification_id)
        {
            if (notification_id == 0)
                throw new MissingParameterException("notification_id");


            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Mark notification as deleted");

            _endUserMessageService.DeleteNotification(notification_id, _identityService.UserId);

            return new BaseResponse() { ErrorCode = (int)ErrorCodes.Success };
        }
        #endregion
    }
}