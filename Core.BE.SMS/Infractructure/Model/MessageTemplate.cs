using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.Model
{
    public class MessageTemplate : Entity
    {
        public MessageTemplate()
        {

        }
        public MessageTemplate(string createdBy)
        {
            Code = $"MT-{new Random().Next(1, 10000)}{DateTime.UtcNow.ToString("yyMMddhhmmss")}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;
        }

        public string ContentEn { set; get; }
        public string ContentAr { set; get; }
        public string ContentSw { set; get; }
        public string TemplateName { set; get; }
    }
}
