using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public interface IMessageRepository : IRepository<Message>
    {
        //Message send(string user_id, MessageType type, List<string> display_params, List<string> hidden_params, string message);
        //Message send(List<string> users, MessageType type, List<string> display_params, List<string> hidden_params, string message);
        Message GetMessageByCode(string code);
        List<Message> GetMessagesByCodes(List<string> code);
        List<Message> GetMessagesByUserId(string userId);
        //List<Message> GetAdminNotificationsByCriteria(GetAdminMessagesCriteriaViewModel criteria);
        List<AdminConversationResponseModel> GetAdminConversationsByCriteria(GetAdminConversationsCriteriaViewModel criteria);
        List<Message> GetAdminConversationDetailsByCriteria(GetAdminConversationDetailsCriteriaViewModel criteria);
        List<Message> GetEndUserConversationDetailsByCriteria(GetEndUserConversationDetailsCriteriaViewModel criteria, string userId);
        List<EndUserConversationsResponseModel> GetEndUserConversationsByCriteria(GetEndUserConversationsCriteriaViewModel criteria, string userId);
        Message GetMessageByMessageId(int messageId);

        //List<Message> GetEndUserNotificationsByCriteria(GetEndUserNotificationsCriteriaViewModel criteria,string userId);
        int GetEndUserUnreadMessagesCount(string userId);
        List<MessageStatus> GetMessageDetails(string messageCode);
        PagedList<Message> GetEndUserMessagesByCriteria(GetNotificationsCriteriaViewModel criteria);
        //GetNotificationsResponse GetNotificationsByCriteriaWithCount(GetNotificationsCriteriaViewModel searchCriteria, string language);
        MessageStatusByEnduser MapStatusToEndUserStatus(Status status);
        PagedList<Message> GetAdminMessages(PaginationVm pagination, SortingViewModel sorting);
    }
}
