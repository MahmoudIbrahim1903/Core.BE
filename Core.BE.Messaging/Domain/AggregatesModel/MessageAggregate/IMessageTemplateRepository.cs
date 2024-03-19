using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public interface IMessageTemplateRepository : IRepository<MessageTemplate>
    {
        MessageTemplate GetMessageTemplateByTemplateId(int templateId);
    }
}
