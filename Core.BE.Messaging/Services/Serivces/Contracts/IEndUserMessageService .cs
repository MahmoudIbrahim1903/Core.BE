using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Services.Serivces.Contracts
{
    public interface IEndUserMessageService
    {
        Task<BaseResponse> UnRegisterFromPush(string userId);
        Task<Response<bool>> RegisterForPush(string userId, string platform, string pushToken, string deviceId, string languageCode);
        int GetEndUserUnreadMessagesCount();
        //GetNotificationsResponse GetMessagesByCriteriaWithCount(GetNotificationsCriteriaViewModel criteria, string language);
        PagedList<EndUserNotificationResponseModel> GetMessagesForEndUser(GetNotificationsCriteriaViewModel getNotificationsCriteriaViewModel, Language language);
        void UpdateMessagesStatus(List<UpdateMessageStatusViewModel> messagesStatus, string userId, string errorReason = null);
        void MarkAllMessagesAsRead(string userId);
        void UpdateMessagesStatus(List<UpdateMessageStatusViewModelV2> notificationsStatus, string userId, string errorReason = null);
        void DeleteNotification(int notificationId, string userId);
    }
}
