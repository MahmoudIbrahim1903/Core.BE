using System;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.ApplicationAggregate
{
    public class ClientApplicationVersion : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)

        public string Name { get; set; }
        public string TerminalTypeCode { get; set; }
        public string Description { get; set; }
        public string ApplicationName { get; set; }
        public string DownloadUrl { get; set; }
        public string Version { get; set; }
        public ClientApplicationVersion()
        {
            Code = $"CV-{new Random().Next(1, 100000)}";
            CreatedBy = "Nasser";
            CreationDate = DateTime.UtcNow;
        }
    }
}
