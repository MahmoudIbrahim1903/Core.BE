using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.Repositories
{
    public class MessageDestinationRepository : BaseRepository<MessageDestination, MessagingContext>, IMessageDestinationRepository
    {
        public MessageDestinationRepository(MessagingContext context) : base(context)
        {
        }

        public void AddNewMessageDestination(Message message, DestinationType destinationType, string destination, string displayName)
        {
            
        }
    }
}
