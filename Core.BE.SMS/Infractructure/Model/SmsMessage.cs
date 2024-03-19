using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.SMS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Emeint.Core.BE.SMS.Infractructure.Model
{
    public class SmsMessage : Entity
    {
        public SmsMessage()
        {

        }

        public SmsMessage(string createdBy)
        {
            Code = $"SMS-{new Random().Next(1, 10000)}{DateTime.UtcNow.ToString("yyMMddhhmmss")}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;
        }
        public string MessageText { set; get; }
        public SmsProvider SmsProviderType { set; get; }
        public int MessageTemplateId { set; get; }

        public virtual ICollection<MessageUser> MessageUsers { set; get; }
        public virtual MessageTemplate MessageTemplate { set; get; }
    }
}