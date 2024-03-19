using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;
using Emeint.Core.BE.Notifications.Domain.Enums;
using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;
using Emeint.Core.BE.Notifications.Services.Serivces.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Services.Serivces.Concretes
{
    public class EndUserMessageService : IEndUserMessageService
    {
        private readonly IMessageAggregateService _messageAggregateService;
        public EndUserMessageService(IMessageAggregateService messageAggregateService)
        {
            _messageAggregateService = messageAggregateService;
        }

        public int GetEndUserUnreadMessagesCount()
        {
            return _messageAggregateService.GetEndUserUnreadMessagesCount();
        }

        //public GetNotificationsResponse GetMessagesByCriteriaWithCount(GetNotificationsCriteriaViewModel criteria, string language)
        //{
        //    return _messageAggregateService.GetMessagesByCriteriaWithCount(criteria, language);
        //}

        public PagedList<EndUserNotificationResponseModel> GetMessagesForEndUser(GetNotificationsCriteriaViewModel getNotificationsCriteriaViewModel, Language language)
        {
            return _messageAggregateService.GetMessagesForEndUser(getNotificationsCriteriaViewModel, language);
        }

        public void MarkAllMessagesAsRead(string userId)
        {
            _messageAggregateService.UpdateMessagesStatusToRead(userId);
        }

        public Task<Response<bool>> RegisterForPush(string userId, string platform, string pushToken, string deviceId, string languageCode)
        {
            return _messageAggregateService.RegisterForPush(userId, platform, pushToken, deviceId, languageCode);
        }

        public async Task<BaseResponse> UnRegisterFromPush(string userId)
        {
            return await _messageAggregateService.UnRegisterFromPush(userId);
        }

        public void UpdateMessagesStatus(List<UpdateMessageStatusViewModel> messagesStatus, string userId, string errorReason = null)
        {
            foreach (var messageStatus in messagesStatus)
            {
                Status status;
                if (messageStatus.Status == MessageStatusByEnduser.Sent)
                    throw new InvalidOperationException("Cannot Update Message Status to Sent");

                else if (messageStatus.Status == MessageStatusByEnduser.Delivered)
                    status = Status.Delivered;

                else
                    status = Status.Read;


                _messageAggregateService.UpdateMessageStatus(messageStatus.MessageCode, userId, status, errorReason);
            }
        }

        public void UpdateMessagesStatus(List<UpdateMessageStatusViewModelV2> notificationsStatus, string userId, string errorReason = null)
        {
            foreach (var notificationStatus in notificationsStatus)
            {
                Status status;
                if (notificationStatus.Status == MessageStatusByEnduser.Sent)
                    throw new InvalidOperationException("Cannot Update Message Status to Sent");

                else if (notificationStatus.Status == MessageStatusByEnduser.Delivered)
                    status = Status.Delivered;

                else
                    status = Status.Read;


                _messageAggregateService.UpdateMessageStatus(notificationStatus.NotificationId, userId, status, errorReason);
            }
        }

        public void DeleteNotification(int notificationId, string userId)
        {
            _messageAggregateService.DeleteNotification(notificationId, userId);
        }
    }
}
