using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public class MessageExtraParam : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)

        public string Key { get; set; }
        public string Value { get; set; }
        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; }

        public MessageExtraParam(string key, string value, Message message, string createdBy)
        {
            var today = DateTime.UtcNow;
            Code = $"E-{today.Minute}-{today.Hour}-{today.Day}-{today.Month}-{today.Year}-{new Random().Next(1, 10000000)}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;

            Key = key;
            Value = value;
            Message = message;
        }
        public MessageExtraParam()
        {
        }
    }
}
