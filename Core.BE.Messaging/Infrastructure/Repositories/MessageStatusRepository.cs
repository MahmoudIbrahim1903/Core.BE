using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.Repositories
{
    public class MessageStatusRepository : BaseRepository<MessageStatus, MessagingContext>, IMessageStatusRepository
    {
        public MessageStatusRepository(MessagingContext context) : base(context)
        {
        }
        public MessageStatus GetMessageStatus(string messageCode, string userId)
        {
            // TODO: Handle disposed context
            return _context.Set<MessageStatus>()
                .Include(m => m.User)
                .Include(m => m.Message)
                .FirstOrDefault(n => n.Message.Code == messageCode && n.User.ApplicationUserId == userId);
        }

        public MessageStatus GetMessageStatus(int messageId, string userId)
        {
            return _context.Set<MessageStatus>()
                .Include(m => m.User)
                .Include(m => m.Message)
                .FirstOrDefault(n => n.Message.Id == messageId && n.User.ApplicationUserId == userId);
        }
        public List<MessageStatus> GetMessagesStatusToMarkAsRead(string userId)
        {
            return _context.Set<MessageStatus>()
                .Include(m => m.User)
                .Where(n => n.User.ApplicationUserId == userId && n.Status != Domain.Enums.Status.Read)
                .ToList();
        }
    }
}