using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
//using Emeint.Core.BE.Notifications.API.Application.Commands;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Emeint.Core.BE.Notifications.Domain.Enums;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate
{
    public class Message : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)

        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string TitleSw { get; set; }
        public string ContentEn { get; set; }
        public string ContentAr { get; set; }
        public string ContentSw { get; set; }
        [ForeignKey("TypeId")]
        public virtual MessageType Type { get; set; } //booking request,National ID verified,Car license verified
        public string SenderId { get; set; } // msg source
        public virtual MessageSource MessageSource { get; set; }
        public bool IsDeleted { get; set; }
        public string Notes { get; set; }
        public List<MessageStatus> Statuses { get; set; }
        public virtual ICollection<MessageExtraParam> MessageExtraParams { get; set; }
        public virtual ICollection<MessageDestination> MessageDestinations { get; set; }

        protected Message()
        {
            MessageExtraParams = new HashSet<MessageExtraParam>();
        }

        public Message(string titleAr, string titleEn, string titleSw, MessageType type, string content, string contentAr, string contentSw, MessageSource messageSource,
            string createdBy, string notes, bool isDeleted = false)
        {
            var today = DateTime.UtcNow;
            Code = $"M-{today.Minute}-{today.Hour}-{today.Day}-{today.Month}-{today.Year}-{new Random().Next(1, 10000000)}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;
            TitleAr = titleAr;
            TitleEn = titleEn;
            TitleSw = titleSw;
            ContentEn = content;
            ContentAr = contentAr;
            ContentSw = contentSw;
            Type = type;
            IsDeleted = isDeleted;
            MessageSource = messageSource;
            Notes = notes;
            MessageExtraParams = new HashSet<MessageExtraParam>();
        }

        public string AddMessageExtraParams(string key, string value, string createdBy)
        {
            var msgExtraParams = new MessageExtraParam(key, value, this, createdBy);

            MessageExtraParams.Add(msgExtraParams);

            return msgExtraParams.Code;
        }

        //public Message()
        //{
        //}
    }
}
