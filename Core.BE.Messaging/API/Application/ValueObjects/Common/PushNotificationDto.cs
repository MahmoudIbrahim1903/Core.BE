using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System.Collections.Generic;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    public class PushNotificationDto
    {
        public int Id { get; set; }
        //public string Title { get; set; }
        public List<PushMessageFinalDestination> Destinations { get; set; }
        public MessageSource MessageSource { get; set; }
        public Dictionary<string, string> ExtraParams { get; set; }
        public string MessageContentEn { get; set; }
        public string MessageContentAr { get; set; }
        public string MessageTitleEn { get; set; }
        public string MessageTitleAr { get; set; }
        public string MessageContentSw { get; set; }
        public string MessageTitleSw { get; set; }
        public bool Alert { get; set; }
        public string MessageTypeCode { get; set; }

        public PushNotificationDto(Message message)
        {
            Id = message.Id;
            //Title = message.TitleEn;
            //DevicePlatform = message.DevicePlatform;
            MessageTypeCode = message.Type.Code;
            MessageSource = message.MessageSource;
        }

        public PushNotificationDto()
        {
            Destinations = new List<PushMessageFinalDestination>();
            ExtraParams = new Dictionary<string, string>();
        }

        public PushNotificationDto(PushNotificationDto pushNotificationDto)
        {
            Destinations = pushNotificationDto.Destinations;
            MessageSource = pushNotificationDto.MessageSource;
            MessageContentEn = pushNotificationDto.MessageContentEn;
            MessageContentAr = pushNotificationDto.MessageContentAr;
            MessageTitleEn = pushNotificationDto.MessageTitleEn;
            MessageTitleAr = pushNotificationDto.MessageTitleAr;
            ExtraParams = pushNotificationDto.ExtraParams;
            MessageTypeCode = pushNotificationDto.MessageTypeCode;
            Alert = pushNotificationDto.Alert;
            Id = pushNotificationDto.Id;
        }
    }

    public class PushMessageFinalDestination
    {
        public string UserId { get; set; }
        public Domain.Enums.Platform Platform { get; set; }
        public string DeviceId { get; set; }
        public string PushToken { get; set; }
        public string Language { get; set; }
    }
}