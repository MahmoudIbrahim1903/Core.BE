using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs
{
    public class MessageDTO
    {
        public int MessageId { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string TitleSw { get; set; }
        public string ContentEn { get; set; }
        public string ContentAr { get; set; }
        public string ContentSw { get; set; }
        public int MessageTypeId { get; set; }
        public string SenderId { get; set; }
        public MessageSource MessageSource { get; set; }
        public bool IsDeleted { get; set; }
        public string Code { get; set; }
        public string TypeCode { get; set; }
        public string CategoryCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string Notes { get; set; }

        public MessageDTO(Message message)
        {
            MessageId = message.Id;
            TitleAr = message.TitleAr;
            TitleEn = message.TitleEn;
            TitleSw = message.TitleSw;
            ContentAr = message.ContentAr;
            ContentEn = message.ContentEn;
            ContentSw = message.ContentSw;
            MessageTypeId = message.Type != null ? message.Type.Id : 0;
            SenderId = message.SenderId;
            MessageSource = message.MessageSource;
            IsDeleted = message.IsDeleted;
            Code = message.Code;
            TypeCode = message.Type?.Code;
            CreatedBy = message.CreatedBy;
            CreationDate = message.CreationDate;
            Notes = message.Notes;
        }
    }
}
