using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs
{
    public class MessageStatusDTO 
    {
        public Status Status { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? FailedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public int DevicePlatformId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorReason { get; set; }
        public string UserId { get; set; }
        public int UserDeviceId { get; set; }
        public int MessageId { get; set; }
        public string Code { get; set; }

        public MessageStatusDTO(MessageStatus messageStatus)
        {
            Status = messageStatus.Status;
            SentDate = messageStatus.SentDate;
            FailedDate = messageStatus.FailedDate;
            DeliveredDate = messageStatus.DeliveredDate;
            ReadDate = messageStatus.ReadDate;
            DevicePlatformId = messageStatus.DevicePlatform != null ? messageStatus.DevicePlatform.Id : 0;
            ErrorCode = messageStatus.ErrorCode;
            ErrorReason = messageStatus.ErrorReason;
            UserId = messageStatus.User != null ? messageStatus.User.ApplicationUserId : string.Empty;
            UserDeviceId = messageStatus.UserDevice != null ? messageStatus.UserDevice.Id : 0;
            MessageId = messageStatus.Message != null ? messageStatus.Id : 0;
            Code = messageStatus.Code;
        }
    }
}
