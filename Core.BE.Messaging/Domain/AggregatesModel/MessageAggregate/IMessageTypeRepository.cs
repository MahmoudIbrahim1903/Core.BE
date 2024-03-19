using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public interface IMessageTypeRepository : IRepository<MessageType>
    {
        List<MessageType> GetMessageTypes();
        MessageType GetMessageTypeByCode(string code);
        MessageType GetMessageTypeById(int id);
    }
}
