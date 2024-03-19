using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Mailing.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Emeint.Core.BE.Mailing.Domain.AggregatesModel
{
    public class Mail : Entity, IAggregateRoot
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? HasAttachments { get; set; }
        public bool? IsImportant { get; set; }
        public string Type { get; set; }
        public string FromUserId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public Status Status { get; set; }


        public Mail(string from, string to, string cc, string subject, string body, bool? hasAttachments, bool? isImportant,
            string type, string fromUserId, string errorCode, string errorMessage, Status status)
        {
            Code = $"M-{Guid.NewGuid()}";
            CreatedBy = "Send Email API";
            CreationDate = DateTime.UtcNow;
            From = from;
            To = to;
            Cc = cc;
            Subject = subject;
            Body = body;
            IsImportant = isImportant;
            HasAttachments = hasAttachments;
            Type = type;
            FromUserId = fromUserId;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Status = status;
        }
    }
}
