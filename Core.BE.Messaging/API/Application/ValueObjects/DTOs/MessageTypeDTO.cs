using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs
{
    public class MessageTypeDTO
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
        public MessageTemplateDTO MessageTemplate { get; set; }

        public MessageTypeDTO(MessageType messageType)
        {
            TypeId = messageType.Id;
            Name = messageType.Name;
            MessageTemplate = new MessageTemplateDTO(messageType.MessageTemplate);
        }
    }
}
