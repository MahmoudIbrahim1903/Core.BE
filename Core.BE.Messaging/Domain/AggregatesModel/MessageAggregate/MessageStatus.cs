using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
//using Emeint.Core.BE.Notifications.Domain.Enums;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public class MessageStatus : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)

        public Status Status { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? FailedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReadDate { get; set; }
        [ForeignKey("DevicePlatformId")]
        public virtual DevicePlatform DevicePlatform { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorReason { get; set; }
        public bool IsDeleted { get; set; }

        public string UserApplicationUserId { set; get; }
        [ForeignKey("UserApplicationUserId")]
        public User User { get; set; }

        [ForeignKey("UserDeviceId")]
        public virtual UserDevice UserDevice { get; set; }
        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; }


        public MessageStatus(Status name, DateTime? sentDate, DateTime? failedDate, DateTime? deliveredDate, DateTime? readDate,
            DevicePlatform devicePlatform, string errorCode, string errorReason, User user, UserDevice userDevice, Message message, string createdBy)
        {
            var today = DateTime.UtcNow;
            Code = $"S-{today.Minute}-{today.Hour}-{today.Day}-{today.Month}-{today.Year}-{new Random().Next(1, 10000000)}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;

            Status = name;
            SentDate = sentDate;
            FailedDate = failedDate;
            DeliveredDate = deliveredDate;
            ReadDate = readDate;
            DevicePlatform = DevicePlatform;
            ErrorCode = errorCode;
            ErrorReason = errorReason;
            User = user;
            UserDevice = userDevice;
            Message = message;
        }
        public MessageStatus()
        {
        }

    }
}
