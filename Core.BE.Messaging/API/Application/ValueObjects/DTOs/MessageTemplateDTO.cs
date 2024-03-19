using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs
{
    public class MessageTemplateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentEn { get; set; }
        public string ContentAr { get; set; }
        public string ContentSw { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string TitleSw { get; set; }
        public string Code { get; set; }

        public MessageTemplateDTO(MessageTemplate messageTemplate)
        {
            Id = messageTemplate.Id;
            Name = messageTemplate.Name;
            ContentAr = messageTemplate.ContentAr;
            ContentEn = messageTemplate.ContentEn;
            ContentSw = messageTemplate.ContentSw;
            TitleAr = messageTemplate.TitleAr;
            TitleEn = messageTemplate.TitleEn;
            TitleSw = messageTemplate.TitleSw;
            Code = messageTemplate.Code;
        }
    }
}
