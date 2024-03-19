using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public class MessageTemplate : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)

        public string Name { get; set; }
        public string ContentEn { get; set; }
        public string ContentAr { get; set; }
        public string ContentSw { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string TitleSw { get; set; }
    }
}
