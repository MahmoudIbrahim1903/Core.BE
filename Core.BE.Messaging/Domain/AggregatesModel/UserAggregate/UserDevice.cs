using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
//using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
//using Emeint.Core.BE.Notifications.Domain.AggregatesModel.PlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate
{
    public class UserDevice : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)

        public string Name { get; set; }
        //public User User { get; set; }
        public Enums.Platform DevicePlatform { get; set; }
        public string Platform { get; set; }
        public string PlatformVersion { get; set; }
        public string Vendor { get; set; }

        public UserDevice(string name, User user, Enums.Platform devicePlatform, string platform, string platformVersion, string vendor)
        {
            Code = $"UD-{new Random().Next(1, 100000)}";
            CreatedBy = "Nasser";
            CreationDate = DateTime.UtcNow;

            //User = user;
            Name = name;
            Platform = platform;
            DevicePlatform = devicePlatform;
            PlatformVersion = platformVersion;
            Vendor = vendor;
        }
        public UserDevice()
        {

        }
    }
}
