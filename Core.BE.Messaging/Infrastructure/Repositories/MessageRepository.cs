using MessageAggregate = Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.EndUser;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Utilities;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using System.Linq.Expressions;

namespace Emeint.Core.BE.Notifications.Infrastructure.Repositories
{
    public class MessageRepository : BaseRepository<Message, MessagingContext>, IMessageRepository
    {
        List<Message> result = new List<Message>();
        public MessageRepository(MessagingContext context) : base(context)
        {

        }

        public Message GetMessageByCode(string code)
        {
            return _context.Set<Message>().Where(n => n.Code == code).FirstOrDefault();
        }

        public List<Message> GetMessagesByReceiverId(string userId)
        {
            result = _context.Set<Message>().Include(m => m.Type).Include(m => m.MessageDestinations)
                .Where(m => m.MessageDestinations.Contains(m.MessageDestinations
                .FirstOrDefault(d => (d.DestinationType == DestinationType.All)
                || (d.DestinationType == DestinationType.User && d.Destination == userId)
                //|| (d.DestinationType == DestinationType.Group && IsGroupMemeber(d.Destination, userId))
                ))).ToList();
            //TODO: add group destination after handle groups 
            return result;
        }

        private bool IsGroupMemeber(string destination, string userId)
        {
            //TODO: check users belongs to a group after adding groups  
            return true;
        }

        public List<Message> GetEndUserConversationDetailsByCriteria(GetEndUserConversationDetailsCriteriaViewModel conversationCriteria, string userId)
        {
            var pageSize = conversationCriteria?.Pagination?.PageSize ?? 10;
            var pageNumber = (int)((conversationCriteria.Pagination?.PageNumber - 1) * conversationCriteria.Pagination?.PageSize);
            var direction = (conversationCriteria.Criteria.Direction != null) ? conversationCriteria.Criteria.Direction : Direction.next;
            var currentMessageId = (conversationCriteria.Criteria.CurrentMessageId != null) ? conversationCriteria.Criteria.CurrentMessageId : null;
            Message currentMessage = null;
            if (currentMessageId != null && currentMessageId != 0)
            {
                currentMessage = GetMessageByMessageId((int)currentMessageId);
            }

            List<Message> messages = new List<Message>();

            if (conversationCriteria.Pagination != null && conversationCriteria.Pagination.PageSize != null && conversationCriteria.Pagination.PageNumber != null && conversationCriteria.Pagination.PageSize > 0)
            {
                if (currentMessage != null)
                {
                    if (direction == Direction.next)
                    {
                        messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
                                           .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                      //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                      m.MessageSource == MessageSource.EndUserMessage &&
                                                      //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId))
                                                      ((m.Statuses.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault() && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                       //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId))
                                                       || (m.SenderId == userId && m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.PeerId)).FirstOrDefault()))
                                                      && m.Id > currentMessage.Id)
                                                      .Skip(pageNumber).Take(pageSize).ToList();
                    }
                    else
                    {
                        messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
                                           .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                      //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                      m.MessageSource == MessageSource.EndUserMessage &&
                                                      //((m.Status.FirstOrDefault().User.ApplicationUserId == userId && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                      //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId))
                                                      ((m.Statuses.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault() && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                       //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId))
                                                       || (m.SenderId == userId && m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.PeerId)).FirstOrDefault()))
                                                      && m.Id < currentMessage.Id)
                                                      .Skip(pageNumber).Take(pageSize).ToList();
                    }
                }
                else
                {
                    messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
                                           .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                      //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                      m.MessageSource == MessageSource.EndUserMessage &&
                                                      //((m.Status.FirstOrDefault().User.ApplicationUserId == userId && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                      //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId) )
                                                      ((m.Statuses.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault() && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                       //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId))
                                                       || (m.SenderId == userId && m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.PeerId)).FirstOrDefault()))
                                                      ).Skip(pageNumber).Take(pageSize).ToList();
                }
            }
            else
            {
                if (currentMessage != null)
                {
                    if (direction == Direction.next)
                    {
                        messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
                                           .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                      //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                      m.MessageSource == MessageSource.EndUserMessage &&
                                                      //((m.Status.FirstOrDefault().User.ApplicationUserId == userId && m.SenderId == conversationCriteria.Criteria.PeerId) 
                                                      //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId))
                                                      ((m.Statuses.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault() && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                       //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId))
                                                       || (m.SenderId == userId && m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.PeerId)).FirstOrDefault()))
                                                      && m.Id > currentMessage.Id).ToList();
                    }
                    else
                    {
                        messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
                                           .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                      //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                      m.MessageSource == MessageSource.EndUserMessage &&
                                                       //((m.Status.FirstOrDefault().User.ApplicationUserId == userId && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                       ((m.Statuses.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault() && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                       //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId))
                                                       || (m.SenderId == userId && m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.PeerId)).FirstOrDefault()))
                                                      && m.Id < currentMessage.Id).ToList();
                    }
                }
                else
                {
                    messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
                                            .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                       //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                       m.MessageSource == MessageSource.EndUserMessage &&
                                                       //((m.Status.FirstOrDefault().User.ApplicationUserId == userId && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                       ((m.Statuses.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault() && m.SenderId == conversationCriteria.Criteria.PeerId)
                                                       //|| (m.SenderId == userId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.PeerId))
                                                       || (m.SenderId == userId && m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.PeerId)).FirstOrDefault()))
                                                       ).ToList();
                }
            }
            return messages;
        }

        public List<EndUserConversationsResponseModel> GetEndUserConversationsByCriteria(GetEndUserConversationsCriteriaViewModel conversationCriteria, string userId)
        {
            var pageSize = conversationCriteria?.Pagination?.PageSize ?? 10;
            var pageNumber = (int)((conversationCriteria.Pagination?.PageNumber - 1) * conversationCriteria.Pagination?.PageSize);
            List<EndUserConversationsResponseModel> conversations = new List<EndUserConversationsResponseModel>();
            List<EndUserConversationsResponseModel> orderedConversations = new List<EndUserConversationsResponseModel>();
            EndUserConversationsResponseModel conversation = new EndUserConversationsResponseModel();
            ConversationMessageViewModel messageContentObj = new ConversationMessageViewModel();
            List<Message> messages = new List<Message>();

            if (conversationCriteria.Pagination != null && conversationCriteria.Pagination.PageSize != null && conversationCriteria.Pagination.PageNumber != null && conversationCriteria.Pagination.PageSize > 0)
            {
                //all messages sent and received
                messages = _context.Set<Message>().Include(m => m.Statuses).ThenInclude(b => b.User)
                                       .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                  //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                  m.MessageSource == MessageSource.EndUserMessage &&
                                                  //(m.Status.FirstOrDefault().User.ApplicationUserId == userId || m.SenderId == userId)
                                                  (m.Statuses.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault() || m.SenderId == userId)

                                                  ).Skip(pageNumber).Take(pageSize)
                                                  .ToList();
            }
            else
            {
                messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).ThenInclude(b => b.User).Include(m => m.MessageExtraParams)
                                            .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                       //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                       m.MessageSource == MessageSource.EndUserMessage &&
                                                       //(m.Status.FirstOrDefault().User.ApplicationUserId == userId || m.SenderId == userId)
                                                       (m.Statuses.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault() || m.SenderId == userId)

                                                       ).ToList();
            }

            foreach (var msg in messages)
            {
                conversation = conversations.Find(x => x.PeerId == msg.Statuses.FirstOrDefault().User.ApplicationUserId || x.PeerId == msg.SenderId);
                if (conversation == null)
                {
                    conversation = new EndUserConversationsResponseModel()
                    {
                        Messages = new List<ConversationMessageViewModel>(),
                        PeerId = ""
                    };
                    messageContentObj = new ConversationMessageViewModel()
                    {
                        DeliveredDate = "",
                        SenderId = "",
                        MessageBody = ""
                    };
                    messageContentObj.DeliveredDate = (msg.Statuses != null && msg.Statuses.Count() > 0) ? msg.Statuses.Select(a => a.DeliveredDate?.ToString("yyyyMMddHHmmss")).FirstOrDefault() : (DateTime.UtcNow).ToString("yyyyMMddHHmmss");
                    messageContentObj.SenderId = msg.SenderId;
                    messageContentObj.MessageBody = msg.ContentEn;

                    conversation.Messages.Add(messageContentObj);
                    conversation.PeerId = msg.SenderId;
                    conversations.Add(conversation);
                }
                else
                {
                    conversation = conversations.Find(x => msg.Statuses.Select(a => a.User.ApplicationUserId.Contains(x.PeerId)).FirstOrDefault() || x.PeerId == msg.SenderId);

                    messageContentObj = new ConversationMessageViewModel()
                    {
                        DeliveredDate = "",
                        SenderId = "",
                        MessageBody = ""
                    };
                    messageContentObj.DeliveredDate = (msg.Statuses != null && msg.Statuses.Count() > 0) ? msg.Statuses.Select(a => a.DeliveredDate?.ToString("yyyyMMddHHmmss")).FirstOrDefault() : (DateTime.UtcNow).ToString("yyyyMMddHHmmss");
                    messageContentObj.SenderId = msg.SenderId;
                    messageContentObj.MessageBody = msg.ContentEn;
                    conversation.Messages.Add(messageContentObj);
                }
            }

            foreach (var item in conversations)
            {
                conversation = new EndUserConversationsResponseModel()
                {
                    Messages = new List<ConversationMessageViewModel>(),
                    PeerId = ""
                };
                conversation.Messages = item.Messages.AsEnumerable().Reverse().ToList();
                conversation.PeerId = item.PeerId;
                orderedConversations.Add(conversation);
            }

            return orderedConversations.AsEnumerable().Reverse().ToList();
        }

        //public List<Message> GetEndUserNotificationsByCriteria(GetEndUserNotificationsCriteriaViewModel criteria, string userId)
        //{
        //    //var pageSize = criteria?.Pagination?.PageSize ?? 10;
        //    //var pageNumber = criteria?.Pagination?.PageNumber != null ? (int)((criteria?.Pagination?.PageNumber - 1) * criteria?.Pagination?.PageSize) : 0;
        //    //var from = (criteria.from != null) ? criteria.from : DateTime.MinValue;
        //    //var to = (criteria.to != null) ? criteria.to : DateTime.UtcNow;

        //    var pageSize = criteria?.Pagination?.PageSize ?? 10;
        //    //var pageNumber = (int)((criteria.Pagination?.PageNumber - 1) * criteria.Pagination?.PageSize);
        //    var pageNumber = criteria?.Pagination?.PageNumber != null ? (int)((criteria?.Pagination?.PageNumber - 1) * criteria?.Pagination?.PageSize) : 0;
        //    var direction = (criteria.Direction != null) ? criteria.Direction : Direction.next;
        //    var currentMessageCode = (criteria.CurrentMessageCode != null) ? criteria.CurrentMessageCode : null;
        //    Message current_message = null;
        //    if (currentMessageCode != null && currentMessageCode != "")
        //    {
        //        current_message = GetMessageByCode(currentMessageCode);
        //    }

        //    List<Message> notifications = new List<Message>();

        //    if (criteria.Pagination != null && criteria.Pagination.PageSize != null && criteria.Pagination.PageNumber != null && criteria.Pagination.PageSize > 0)
        //    {
        //        if (current_message != null)
        //        {
        //            if (direction == Direction.next)
        //            {
        //                notifications = _context.Set<Message>().Include(m => m.Type).Include(m => m.Status).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
        //                                   .Where(m => (criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
        //                                             (criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
        //                                              m.MessageSource != MessageSource.EndUserMessage && m.Status.FirstOrDefault().User.ApplicationUserId == userId
        //                                              && m.Id > current_message.Id)
        //                                              .Skip(pageNumber).Take(pageSize).ToList();
        //            }
        //            else
        //            {
        //                notifications = _context.Set<Message>().Include(m => m.Type).Include(m => m.Status).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
        //                                   .Where(m => (criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
        //                                             (criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
        //                                              m.MessageSource != MessageSource.EndUserMessage && m.Status.FirstOrDefault().User.ApplicationUserId == userId
        //                                              && m.Id < current_message.Id)
        //                                              .Skip(pageNumber).Take(pageSize).ToList();
        //            }
        //        }
        //        else
        //        {
        //            notifications = _context.Set<Message>().Include(m => m.Type).Include(m => m.Status).ThenInclude(m=>m.User).Include(m => m.MessageExtraParams)
        //                                   .Where(m =>(criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
        //                                             (criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) && 
        //                                              m.MessageSource != MessageSource.EndUserMessage && m.Status.FirstOrDefault().User.ApplicationUserId == userId)
        //                                              .Skip(pageNumber).Take(pageSize).ToList();
        //        }
        //    }
        //    else
        //    {
        //        if (current_message != null)
        //        {
        //            if (direction == Direction.next)
        //            {
        //                                   .Where(m => (criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
        //                                             (criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
        //                                              m.MessageSource != MessageSource.EndUserMessage && m.Status.FirstOrDefault().User.ApplicationUserId == userId
        //                                              && m.Id > current_message.Id).ToList();
        //            }
        //            else
        //            {
        //                notifications = _context.Set<Message>().Include(m => m.Type).Include(m => m.Status).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
        //                                   .Where(m => (criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
        //                                             (criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
        //                                              m.MessageSource != MessageSource.EndUserMessage && m.Status.FirstOrDefault().User.ApplicationUserId == userId
        //                                              && m.Id < current_message.Id).ToList();
        //            }
        //        }
        //        else
        //        {
        //            notifications = _context.Set<Message>().Include(m => m.Type).Include(m => m.Status).ThenInclude(m => m.User).Include(m => m.MessageExtraParams)
        //                                    .Where(m => (criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
        //                                              (criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
        //                                               m.MessageSource != MessageSource.EndUserMessage && m.Status.FirstOrDefault().User.ApplicationUserId == userId)
        //                                               .ToList();
        //        }
        //    }

        //    return notifications.OrderByDescending(m=>m.CreationDate).ToList();
        //}

        public int GetEndUserUnreadMessagesCount(string userId)
        {
            var messages = _context.MessageStatus.Where(s => s.User.ApplicationUserId == userId && s.Status != Status.Read && s.IsDeleted == false);
            return messages.Count();
        }

        public List<Message> GetMessagesByCodes(List<string> codes)
        {
            List<Message> messages = _context.Set<Message>().Where(t => codes.Contains(t.Code)).ToList();
            return messages;
        }

        public List<Message> GetMessagesByUserId(string userId)
        {
            //List<Message> messages = _context.Set<Message>().Include(m=>m.Status).Where(t => userId.Contains(t.Status.FirstOrDefault().User.UserId)).ToList();
            List<Message> messages = _context.Set<Message>().Include(m => m.Statuses).Where(m => m.Statuses.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault()).ToList();
            return messages;
        }
        //(userId == null || m.Status.Select(a => a.User.ApplicationUserId.Contains(userId)).FirstOrDefault()) &&

        //public Message GetMessageDetails(string messageCode, string userId)
        //{
        //    Message messages = _context.Set<Message>().Include(m => m.Status).Where(m => m.Status.FirstOrDefault().User.ApplicationUserId == userId && m.Code== messageCode).FirstOrDefault();
        //    return messages;
        //}

        public List<MessageStatus> GetMessageDetails(string messageCode)
        {
            return _context.Set<MessageStatus>().Include(m => m.Message).Include(m => m.User).Where(m => m.Message.Code == messageCode).ToList();
        }

        #region admin methods
        //public List<Message> GetAdminNotificationsByCriteria(GetAdminMessagesCriteriaViewModel criteria)
        //{
        //    var pageSize = criteria?.Pagination?.PageSize ?? 10;
        //    var pageNumber = criteria?.Pagination?.PageNumber != null ? (int)((criteria?.Pagination?.PageNumber - 1) * criteria?.Pagination?.PageSize) : 0;
        //    //var from = (criteria.from != null) ? criteria.from : DateTime.MinValue;
        //    //var to = (criteria.to != null) ? criteria.to : DateTime.UtcNow;
        //    var userId = (criteria.ToUserId != null) ? criteria.ToUserId : null;

        //    List<Message> notifications = new List<Message>();
        //    if (criteria.Pagination != null && criteria.Pagination.PageSize != null && criteria.Pagination.PageNumber != null && criteria.Pagination.PageSize > 0)
        //    {
        //        notifications = _context.Set<Message>().Include(m => m.Type).Include(m => m.Status).Include(m => m.MessageExtraParams)
        //                                   .Where(m => (criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
        //                                             (criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
        //                                             (userId == null || userId == m.Status.FirstOrDefault().User.ApplicationUserId) &&
        //                                             (criteria.Title == null || m.TitleEn.ToLower().Contains(criteria.Title.ToLower())) &&
        //                                             (criteria.Body == null || m.ContentEn.ToLower().Contains(criteria.Body.ToLower())) &&
        //                                             m.MessageSource != MessageSource.EndUserMessage)
        //                                             .Skip(pageNumber).Take(pageSize).ToList();
        //    }
        //    else
        //    {
        //        notifications = _context.Set<Message>().Include(m => m.Type).Include(m => m.Status).Include(m => m.MessageExtraParams)
        //                                             .Where(m => (criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
        //                                             (criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
        //                                             (userId == null || userId == m.Status.FirstOrDefault().User.ApplicationUserId) &&
        //                                             (criteria.Title == null || m.TitleEn.ToLower().Contains(criteria.Title.ToLower())) &&
        //                                             (criteria.Body == null || m.ContentEn.ToLower().Contains(criteria.Body.ToLower())) &&
        //                                             m.MessageSource != MessageSource.EndUserMessage).ToList();
        //    }

        //    if (criteria.Sorting != null)
        //    {
        //        switch (criteria.Sorting)
        //        {
        //            case SortBy.SentDate:
        //                notifications = criteria.SortDirection == SortDirection.Desc ? notifications.OrderByDescending(v => v.Statuses.FirstOrDefault().SentDate).ToList() : notifications.OrderBy(v => v.Statuses.FirstOrDefault().SentDate).ToList();
        //                break;
        //            case SortBy.DeliveredDate:
        //                notifications = criteria.SortDirection == SortDirection.Desc ? notifications.OrderByDescending(v => v.Statuses.FirstOrDefault().DeliveredDate).ToList() : notifications.OrderByDescending(v => v.Statuses.FirstOrDefault().DeliveredDate).ToList();
        //                break;
        //            case SortBy.CreatedDate:
        //                notifications = criteria.SortDirection == SortDirection.Desc ? notifications.OrderByDescending(v => v.CreationDate).ToList() : notifications.OrderByDescending(v => v.CreationDate).ToList();
        //                break;
        //            default:
        //                notifications = notifications.OrderByDescending(v => v.CreationDate).ToList();
        //                break;
        //        }
        //    }

        //    return notifications;
        //}

        public List<Message> GetAdminConversationDetailsByCriteria(GetAdminConversationDetailsCriteriaViewModel conversationCriteria)
        {
            var pageSize = conversationCriteria?.Pagination?.PageSize ?? 10;
            var pageNumber = conversationCriteria?.Pagination?.PageNumber != null ? (int)((conversationCriteria?.Pagination?.PageNumber - 1) * conversationCriteria?.Pagination?.PageSize) : 0;
            //var pageNumber = (int)((criteria.Pagination?.PageNumber - 1) * criteria.Pagination?.PageSize);

            List<Message> messages = new List<Message>();

            if (conversationCriteria.Pagination != null && conversationCriteria.Pagination.PageSize != null && conversationCriteria.Pagination.PageNumber != null && conversationCriteria.Pagination.PageSize > 0)
            {
                messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).Include(m => m.MessageExtraParams)
                                       .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                 (conversationCriteria.Criteria.Types == null || conversationCriteria.Criteria.Types.Count == 0 || conversationCriteria.Criteria.Types.Contains(m.Type.Code)) &&
                                                  m.MessageSource == MessageSource.EndUserMessage &&
                                                  //((m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.FirstPeerId && m.SenderId == conversationCriteria.Criteria.SecondPeerId)
                                                  ((m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.FirstPeerId)).FirstOrDefault() && m.SenderId == conversationCriteria.Criteria.SecondPeerId)
                                                  //|| (m.SenderId == conversationCriteria.Criteria.FirstPeerId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.SecondPeerId))
                                                  || (m.SenderId == conversationCriteria.Criteria.FirstPeerId && m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.SecondPeerId)).FirstOrDefault()))
                                                  ).Skip(pageNumber).Take(pageSize).ToList();
            }
            else
            {
                messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).Include(m => m.MessageExtraParams)
                                        .Where(m => (conversationCriteria.Criteria.Statuses == null || conversationCriteria.Criteria.Statuses.Count == 0 || conversationCriteria.Criteria.Statuses.Contains(m.Statuses.FirstOrDefault().Status)) &&
                                                  (conversationCriteria.Criteria.Types == null || conversationCriteria.Criteria.Types.Count == 0 || conversationCriteria.Criteria.Types.Contains(m.Type.Code)) &&
                                                   m.MessageSource == MessageSource.EndUserMessage &&
                                                  //((m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.FirstPeerId && m.SenderId == conversationCriteria.Criteria.SecondPeerId)
                                                  ((m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.FirstPeerId)).FirstOrDefault() && m.SenderId == conversationCriteria.Criteria.SecondPeerId)
                                                  //|| (m.SenderId == conversationCriteria.Criteria.FirstPeerId && m.Status.FirstOrDefault().User.ApplicationUserId == conversationCriteria.Criteria.SecondPeerId))
                                                  || (m.SenderId == conversationCriteria.Criteria.FirstPeerId && m.Statuses.Select(a => a.User.ApplicationUserId.Contains(conversationCriteria.Criteria.SecondPeerId)).FirstOrDefault()))
                                                   ).ToList();

            }
            return messages;
        }

        public List<AdminConversationResponseModel> GetAdminConversationsByCriteria(GetAdminConversationsCriteriaViewModel criteria)
        {
            var pageSize = criteria?.Pagination?.PageSize ?? 10;
            var pageNumber = (int)((criteria.Pagination?.PageNumber - 1) * criteria.Pagination?.PageSize);
            List<AdminConversationResponseModel> conversations = new List<AdminConversationResponseModel>();
            List<AdminConversationResponseModel> orderedConversations = new List<AdminConversationResponseModel>();
            AdminConversationResponseModel conversation = new AdminConversationResponseModel();
            ConversationMessageViewModel messageContentObj = new ConversationMessageViewModel();
            List<Message> messages = new List<Message>();

            if (criteria.Pagination != null && criteria.Pagination.PageSize != null && criteria.Pagination.PageNumber != null && criteria.Pagination.PageSize > 0)
            {
                //all messages sent and received
                messages = _context.Set<Message>().Include(m => m.Statuses).ThenInclude(b => b.User)
                                       .Where(m =>
                                                  //(criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
                                                  //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                  m.MessageSource == MessageSource.EndUserMessage
                                                  // && (m.Status.FirstOrDefault().User.ApplicationUserId == userId || m.SenderId == userId)
                                                  ).Skip(pageNumber).Take(pageSize)
                                                  .ToList();

            }
            else
            {
                messages = _context.Set<Message>().Include(m => m.Type).Include(m => m.Statuses).ThenInclude(b => b.User).Include(m => m.MessageExtraParams)
                                            .Where(m =>
                                                       //(criteria.Statuses == null || criteria.Statuses.Count == 0 || criteria.Statuses.Contains(m.Status.FirstOrDefault().Status)) &&
                                                       //(criteria.Types == null || criteria.Types.Count == 0 || criteria.Types.Contains(m.Type.Code)) &&
                                                       m.MessageSource == MessageSource.EndUserMessage
                                                       //&& (m.Status.FirstOrDefault().User.ApplicationUserId == userId || m.SenderId == userId)
                                                       ).ToList();
            }

            foreach (var msg in messages)
            {
                //conversation = conversations.Find(x => x.PeerId == msg.Status.FirstOrDefault().User.ApplicationUserId || x.PeerId == msg.SenderId);
                conversation = conversations.Find(x => msg.Statuses.Select(a => a.User.ApplicationUserId.Contains(x.PeerId)).FirstOrDefault() || x.PeerId == msg.SenderId);

                if (conversation == null)
                {
                    conversation = new AdminConversationResponseModel()
                    {
                        Messages = new List<ConversationMessageViewModel>(),
                        PeerId = ""
                    };
                    messageContentObj = new ConversationMessageViewModel()
                    {
                        DeliveredDate = "",
                        SenderId = "",
                        MessageBody = ""
                    };
                    messageContentObj.DeliveredDate = (msg.Statuses != null && msg.Statuses.Count() > 0) ? msg.Statuses.Select(a => a.DeliveredDate?.ToString("yyyyMMddHHmmss")).FirstOrDefault() : (DateTime.UtcNow).ToString("yyyyMMddHHmmss");
                    messageContentObj.SenderId = msg.SenderId;
                    messageContentObj.MessageBody = msg.ContentEn;

                    conversation.Messages.Add(messageContentObj);
                    conversation.PeerId = msg.SenderId;
                    conversations.Add(conversation);
                }
                else
                {
                    //conversation = conversations.Find(x => x.PeerId == msg.Status.FirstOrDefault().User.ApplicationUserId || x.PeerId == msg.SenderId);
                    conversation = conversations.Find(x => msg.Statuses.Select(a => a.User.ApplicationUserId.Contains(x.PeerId)).FirstOrDefault() || x.PeerId == msg.SenderId);

                    messageContentObj = new ConversationMessageViewModel()
                    {
                        DeliveredDate = "",
                        SenderId = "",
                        MessageBody = ""
                    };
                    messageContentObj.DeliveredDate = (msg.Statuses != null && msg.Statuses.Count() > 0) ? msg.Statuses.Select(a => a.DeliveredDate?.ToString("yyyyMMddHHmmss")).FirstOrDefault() : (DateTime.UtcNow).ToString("yyyyMMddHHmmss");
                    messageContentObj.SenderId = msg.SenderId;
                    messageContentObj.MessageBody = msg.ContentEn;
                    conversation.Messages.Add(messageContentObj);
                }
            }

            foreach (var item in conversations)
            {
                conversation = new AdminConversationResponseModel()
                {
                    Messages = new List<ConversationMessageViewModel>(),
                    PeerId = ""
                };
                conversation.Messages = item.Messages.AsEnumerable().Reverse().ToList();
                conversation.PeerId = item.PeerId;
                orderedConversations.Add(conversation);
            }

            return orderedConversations.AsEnumerable().Reverse().ToList();
        }


        public PagedList<Message> GetEndUserMessagesByCriteria(GetNotificationsCriteriaViewModel searchCriteria)
        {
            var userId = searchCriteria.Criteria.ToUserId ?? null;
            var direction = searchCriteria.Criteria?.Direction;
            var currentMessageId = searchCriteria.Criteria?.CurrentMessageId;
            Message currentMessage = null;
            if (currentMessageId != null)
            {
                currentMessage = GetMessageByMessageId((int)currentMessageId);
            }
            IQueryable<MessageStatus> messagesStatuses = _context
                .MessageStatus
                .Where(ms => ms.IsDeleted == false)
                .Where(ms => ms.SentDate >= searchCriteria.Criteria.SentDateFrom)
                .Include(s => s.Message)
                .Include(s => s.Message.Type)
                .Include(s => s.Message.MessageExtraParams);

            //Filter by status
            if (searchCriteria.Criteria.Statuses != null && searchCriteria.Criteria.Statuses.Count > 0)
                messagesStatuses = messagesStatuses.Where(ms => searchCriteria.Criteria.Statuses.Contains(ms.Status));

            // Filter by types
            if (searchCriteria.Criteria.Types != null && searchCriteria.Criteria.Types.Count > 0)
                messagesStatuses = messagesStatuses.Where(ms => searchCriteria.Criteria.Types.Contains(ms.Message.Type.Code));

            // Filter by user Id
            if (userId != null) // TODO: Get by destination (all, user and userid = userid, group and user is member in group
                messagesStatuses = messagesStatuses.Where(ms => ms.UserApplicationUserId == userId);

            // Filter by title
            if (searchCriteria.Criteria.Title != null)
                messagesStatuses = messagesStatuses.Where(ms => ms.Message.TitleEn.ToLower().Contains(searchCriteria.Criteria.Title.ToLower()));

            // Filter by body
            if (searchCriteria.Criteria.Body != null)
                messagesStatuses = messagesStatuses.Where(ms => ms.Message.ContentEn.ToLower().Contains(searchCriteria.Criteria.Body.ToLower()));

            if (searchCriteria.Criteria.NotificationSources.Count > 0)
            {
                messagesStatuses = messagesStatuses.Where(ms => searchCriteria.Criteria.NotificationSources.Contains(ms.Message.MessageSource));
            }

            if (currentMessage != null)
            {
                messagesStatuses = direction == Direction.next ? messagesStatuses.Where(ms => ms.Message.Id > currentMessage.Id) :
                    messagesStatuses.Where(ms => ms.Message.Id < currentMessage.Id);
            }

            if (searchCriteria.Sorting != null)
            {
                messagesStatuses = searchCriteria.Sorting.SortBy switch
                {
                    SortBy.SentDate => searchCriteria.Sorting.SortDirection == SortDirection.Desc ? messagesStatuses.OrderByDescending(v => v.SentDate) : messagesStatuses.OrderBy(v => v.SentDate),
                    SortBy.DeliveredDate => searchCriteria.Sorting.SortDirection == SortDirection.Desc ? messagesStatuses.OrderByDescending(v => v.DeliveredDate) : messagesStatuses.OrderBy(v => v.DeliveredDate),
                    SortBy.CreatedDate => searchCriteria.Sorting.SortDirection == SortDirection.Desc ? messagesStatuses.OrderByDescending(v => v.CreationDate) : messagesStatuses.OrderBy(v => v.CreationDate),
                    _ => messagesStatuses.OrderByDescending(v => v.CreationDate),
                };
            }
            else
                messagesStatuses = messagesStatuses.OrderByDescending(m => m.CreationDate);

            var notificationsPagedList = new PagedList<Message>(messagesStatuses.Count(), new PaginationVm());

            if (searchCriteria.Pagination != null && searchCriteria.Pagination?.PageSize != null && searchCriteria.Pagination?.PageNumber != null && searchCriteria.Pagination?.PageSize > 0 && searchCriteria.Pagination?.PageNumber > 0)
            {
                notificationsPagedList = new PagedList<Message>(messagesStatuses.Count(), new PaginationVm { PageNumber = searchCriteria.Pagination.PageNumber.Value, PageSize = searchCriteria.Pagination.PageSize.Value });
                messagesStatuses = messagesStatuses.Skip(notificationsPagedList.SkipCount).Take(notificationsPagedList.TakeCount);
            }
            notificationsPagedList.List = messagesStatuses.ToList().Select(ms => ms.Message).ToList();
            return notificationsPagedList;
        }

        public MessageStatusByEnduser MapStatusToEndUserStatus(Status status)
        {
            switch (status)
            {
                case Status.Delivered:
                    return MessageStatusByEnduser.Delivered;
                case Status.Read:
                    return MessageStatusByEnduser.Read;
                default:
                    return MessageStatusByEnduser.Sent;
            }
        }
        #endregion
        public Message GetMessageByMessageId(int messageId)
        {
            return _context.Set<Message>().Include(s => s.Type).FirstOrDefault(n => n.Id == messageId);
        }

        public PagedList<Message> GetAdminMessages(PaginationVm pagination, SortingViewModel sorting)
        {
            IQueryable<Message> messages = _context.Message.Where(m => m.MessageSource == MessageSource.AdminMessage).AsQueryable();

            if (sorting != null)
            {
                messages = sorting.SortBy switch
                {
                    SortBy.CreatedDate => sorting.SortDirection == SortDirection.Desc ? messages.OrderByDescending(m => m.CreationDate) : messages.OrderBy(m => m.CreationDate),
                    _ => messages.OrderByDescending(m => m.CreationDate)
                };
            }
            else
            {
                messages = messages.OrderByDescending(m => m.CreationDate);
            }

            var messagePagedList = new PagedList<Message>(messages.Count(), pagination);

            if (pagination != null)
                messages = messages.Skip(messagePagedList.SkipCount).Take(messagePagedList.TakeCount);

            messagePagedList.List = messages.ToList();

            return messagePagedList;
        }
    }
}
