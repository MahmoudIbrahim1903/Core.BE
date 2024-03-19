using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public interface IMessageStatusRepository : IRepository<MessageStatus>
    {
        MessageStatus GetMessageStatus(string messageCode, string userId);
        MessageStatus GetMessageStatus(int messageId, string userId);
        List<MessageStatus> GetMessagesStatusToMarkAsRead(string userId);

    }
}
