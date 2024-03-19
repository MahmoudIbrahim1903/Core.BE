using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.Domain.Enums;
using Emeint.Core.BE.Notifications.Services.Serivces.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Emeint.Core.BE.Notifications.API.Controllers.Admin
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/notifications/admin/v{version:apiVersion}")]
    public class AdminMessagingController : Controller
    {
        #region Fields
        private readonly ILogger<AdminMessagingController> _logger;
        private readonly string _typeName;
        private readonly IAdminMessageService _adminMessageService;
        #endregion

        #region Constructor
        public AdminMessagingController(ILogger<AdminMessagingController> logger, IAdminMessageService adminMessageService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _typeName = GetType().Name;
            _adminMessageService = adminMessageService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Send Admin message to driver or owner
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Send Message Failed</response>
        [Route("send")]
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, NotificationsFullController")]
        public async Task<BaseResponse> Send([FromBody]AdminSendNotificationRequestVm message)//, [FromHeader(Name = "Language")] string language)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            if (message == null)
                throw new MissingParameterException("message");
            _logger.LogInformation($"{_typeName}.{methodName}: Send Admin message BE Messaging Send");

            SendNotificationRequestDto messageFull = new SendNotificationRequestDto();
            messageFull.DisplayParams.Add(message.ContentEn);
            messageFull.DisplayParamsAr.Add(message.ContentAr);
            messageFull.DisplayParamsSw.Add(message.ContentSw);
            messageFull.TitleParams.Add(message.TitleEn);
            messageFull.TitleParamsAr.Add(message.TitleAr);
            messageFull.TitleParamsSw.Add(message.TitleSw);
            messageFull.Destinations = message.Destinations;
            messageFull.MessageSource = MessageSource.AdminMessage;
            messageFull.MessageTypeCode = message.MessageTypeCode;
            messageFull.Alert = message.Alert;
            messageFull.Notes = message.Notes;
            messageFull.SendAsPush = true;
            var result = await _adminMessageService.Send(messageFull);
            return new BaseResponse() { ErrorCode = result.ErrorCode };
        }

        /// <summary>
        /// Get notifications
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Get Messages Failed</response>
        [Route("notifications")]
        [Authorize(Roles = "SuperAdmin, NotificationsFullController, NotificationsReader")]
        [HttpGet]
        public Response<PagedList<AdminGetNotificationSummaryResponseVm>> GetNotifications(SortBy? sort_by, SortDirection? sort_direction, int page_number, int page_size)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            _logger.LogInformation($"{_typeName}.{methodName}: Getting all notification from repository except chat");


            var pagination = (page_number == 0 || page_size == 0) ? null : new PaginationVm { PageSize = page_size, PageNumber = page_number };

            var sorting = new SortingViewModel { SortBy = sort_by, SortDirection = sort_direction };

            var notifications = _adminMessageService.GetMessages(pagination, sorting);

            var result = new PagedList<AdminGetNotificationSummaryResponseVm>
            {
                List = notifications.List.Select(m => new AdminGetNotificationSummaryResponseVm
                {
                    Title = m.TitleEn,
                    Id = m.MessageId,
                    Code = m.Code,
                    Body = m.ContentEn,
                    MessageTypeCode = m.TypeCode,
                    MessageCategoryCode = (m.CategoryCode != null) ? m.CategoryCode : "",
                    SenderId = m.SenderId,
                    CreatedBy = m.CreatedBy,
                    CreationDate = m.CreationDate.ToString("yyyyMMddHHmmss"),
                    Notes = m.Notes
                }).ToList(),
                TotalCount = notifications.TotalCount,
                HasMore = notifications.HasMore
            };

            return new Response<PagedList<AdminGetNotificationSummaryResponseVm>> { Data = result, ErrorCode = 0 };
        }

        /// <summary>
        /// Get message details
        /// </summary>
        /// <response code="200 (error_code=1)">Internal Server Error</response>
        /// <response code="200 (error_code=110)">Get Message details Failed</response>
        [Route("get_message_details")]
        [Authorize(Roles = "SuperAdmin, NotificationsFullController, NotificationsReader")]
        [HttpPost]
        public async Task<Response<List<AdminMessageStatusResponseViewModel>>> GetMessageDetails([FromBody]AdminMessageDetailsRequestViewModel criteria)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Getting message details from repository");

            var result = _adminMessageService.GetMessageDetails(criteria.MessageCode);

            return new Response<List<AdminMessageStatusResponseViewModel>>() { Data = result };
        }

        //TODO: move this API to AdminUsersController and add new admin rolls control listing users

        ///// <summary>
        ///// Get users
        ///// </summary>
        ///// <response code="200 (error_code=1)">Internal Server Error</response>
        ///// <response code="200 (error_code=110)">Get Users Failed</response>
        //[Route("get_users")]
        //[Authorize(Roles = "SuperAdmin, NotificationsFullController, NotificationsReader")]
        //[HttpGet]
        //public async Task<Response<List<AdminUserResponseViewModel>>> GetUsers(AdminGetUsersRequestViewModel criteria)
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //    _logger.LogInformation($"{_typeName}.{methodName}: Getting Users from repository");

        //    var result = _adminMessageService.GetUsers(criteria);
        //    return new Response<List<AdminUserResponseViewModel>>() { Data = result };
        //}

        #endregion
    }
}