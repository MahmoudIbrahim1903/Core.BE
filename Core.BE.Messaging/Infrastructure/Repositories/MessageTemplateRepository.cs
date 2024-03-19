using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.Repositories
{
    public class MessageTemplateRepository : BaseRepository<MessageTemplate, MessagingContext>, IMessageTemplateRepository
    {
        public MessageTemplateRepository(MessagingContext context) : base(context)
        {
        }
        public MessageTemplate GetMessageTemplateByTemplateId(int messageTemplateId)
        {
            return _context.Set<MessageTemplate>().FirstOrDefault(n => n.Id == messageTemplateId);
        }
    }
}
