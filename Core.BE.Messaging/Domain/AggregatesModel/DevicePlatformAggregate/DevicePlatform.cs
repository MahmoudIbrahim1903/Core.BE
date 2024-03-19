using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate
{
    public class DevicePlatform : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)

        public string Name { get; set; }

        public DevicePlatform()
        {
            Code = $"DP-{new Random().Next(1, 100000)}";
        }
    }
}
