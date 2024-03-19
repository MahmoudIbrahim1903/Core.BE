using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public class MessageDestination : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; }
        public DestinationType DestinationType { get; set; }
        //public UserTag UserTag { get; set; }

        public string Destination { get; set; }

        public MessageDestination(Message message, DestinationType destinationType, string createdBy, string destination)//, UserTag userTag)
        {
            Code = $"DEST-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}-{new Random().Next(1, 100000)}";
            Message = message;
            DestinationType = destinationType;
            CreationDate = DateTime.UtcNow;
            CreatedBy = createdBy;
            Destination = destination;
        }
        public MessageDestination()
        {
        }
    }
}
