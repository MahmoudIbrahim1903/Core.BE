using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public class MessageType : Entity, IAggregateRoot
    {
        public string Name { get; set; }
        [ForeignKey("MessageTemplateId")]
        public virtual MessageTemplate MessageTemplate { get; set; }
        public virtual ICollection<Message> Message { get; set; }
        public MessageType(string name, MessageTemplate messageTemplate)
        {
            Code = $"Ty-{new Random().Next(1, 100000)}";
            CreatedBy = "Nasser";
            CreationDate = DateTime.UtcNow;

            Name = name;
            MessageTemplate = messageTemplate;
        }

        public MessageType()
        {
        }
    }
}
