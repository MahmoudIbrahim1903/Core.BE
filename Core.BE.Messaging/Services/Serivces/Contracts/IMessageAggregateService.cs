using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Manager.Contracts
{
    public interface IMessageAggregateService
    {
        Task<Response<SystemEventNotificationDto>> Send(SendNotificationRequestDto message);
        List<AdminMessageStatusResponseViewModel> GetMessageDetails(string messageCode);
        List<AdminConversationResponseModel> GetAdminConversationsByCriteria(GetAdminConversationsCriteriaViewModel criteria);
        List<AdminConversationMessageDetailsViewModel> GetAdminConversationDetailsByCriteria(GetAdminConversationDetailsCriteriaViewModel criteria);
        PagedList<MessageDTO> GetAdminMessages(PaginationVm pagination, SortingViewModel sorting);
        List<AdminUserResponseViewModel> GetUsers(AdminGetUsersRequestViewModel criteria);
        Task UpdateMessagesStatus(List<int> messageIds, string userId, Status status, string errorReason);
        void UpdateMessageStatus(string messageCode, string userId, Status status, string errorReason);
        void UpdateMessageStatus(int messageId, string userId, Status status, string errorReason);
        //GetNotificationsResponse GetMessagesByCriteriaWithCount(GetNotificationsCriteriaViewModel criteria, string language);
        int GetEndUserUnreadMessagesCount();
        void UpdateMessagesStatusToRead(string userId);
        Task<Response<bool>> RegisterForPush(string userId, string platform, string pushToken, string deviceId, string language_code);

        Task<BaseResponse> UnRegisterFromPush(string userId);

        MessageTypeDTO GetMessageTypeByCode(string typeCode);
        int AddNewMessage(string messageTitleAr, string messageTitleEn, string messageTitleSw, int messageTypeId, string messageContentEn, string messageContentAr, string messageContentSw, MessageSource messageSource, string createdBy, string notes, bool isDeleted = false);
        void AddMessageExtraParams(int massageId, string key, string value, string createdBy);
        int AddMessageStatus(Status name, DateTime? sentDate, DateTime? failedDate, DateTime? deliveredDate, DateTime? readDate, int? devicePlatformId, string errorCode, string errorReason, string userId, int? userDeviceId, int messageId, string createdBy);
        MessageDTO GetMessageByMessageId(int messageId);
        MessageStatusDTO GetMessageStatus(string messageCode, string userId);
        MessageDTO GetMessageByCode(string messageCode);
        MessageTemplateDTO GetMessageTemplateByTemplateId(int templateId);
        PagedList<EndUserNotificationResponseModel> GetMessagesForEndUser(GetNotificationsCriteriaViewModel getNotificationsCriteriaViewModel, Language language);
        List<EndUserConversationMessageDetailsResponseModel> GetEndUserConversationDetailsByCriteria(GetEndUserConversationDetailsCriteriaViewModel criteria, string userId);
        List<EndUserConversationsResponseModel> GetEndUserConversationsByCriteria(GetEndUserConversationsCriteriaViewModel criteria, string userId);
        void AddMessageDestination(int messageId, DestinationType destinationType, string destination);
        void DeleteNotification(int notificationId, string userId);
    }

}
