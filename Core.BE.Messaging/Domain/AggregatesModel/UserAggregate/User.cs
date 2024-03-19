using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.ApplicationAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        public string ApplicationUserId { get; set; }// TODO: ApplicationUserId should be PK
        public string Name { get; set; }
        [ForeignKey("UserDeviceId")]
        public virtual UserDevice UserDevice { get; set; }
        public string DeviceId { get; set; }
        public string LanguageCode { get; set; }
        public string PushNotificationToken { get; set; }
        [ForeignKey("ClientApplicationVersionId")]
        public virtual ClientApplicationVersion ClientApplicationVersion { get; set; }
        //public List<Message> Messages { get; set; }
        public List<MessageStatus> MessageStatuses { get; set; }
        //public UserTag UserTag { get; set; }
        public Enums.Platform DevicePlatform { get; set; }

        public User(string userId, string name, string deviceId, string languageCode, string pushNotificationToken, UserDevice userDevice, Enums.Platform platform, string createdBy)
        {
            Code = $"U-{new Random().Next(1, 100000)}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;

            ApplicationUserId = userId;
            UserDevice = userDevice;
            DevicePlatform = platform;
            Name = name;
            DeviceId = deviceId;
            LanguageCode = languageCode;
            PushNotificationToken = pushNotificationToken;
        }

        public User()
        {

        }
    }
}
