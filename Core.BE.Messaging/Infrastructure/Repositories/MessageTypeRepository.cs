using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.Repositories
{
    public class MessageTypeRepository : BaseRepository<MessageType, MessagingContext>, IMessageTypeRepository
    {
        public MessageTypeRepository(MessagingContext context) : base(context)
        {
        }

        public List<MessageType> GetMessageTypes()
        {
            return _context.Set<MessageType>().Include(m => m.MessageTemplate).ToList();
        }

        public MessageType GetMessageTypeByCode(string code)
        {
            return _context.Set<MessageType>().Include(m => m.MessageTemplate).FirstOrDefault(n => n.Code == code);
        }

        public MessageType GetMessageTypeById(int id)
        {
            return _context.Set<MessageType>().Include(m => m.MessageTemplate).FirstOrDefault(n => n.Id == id);
        }
    }
}
