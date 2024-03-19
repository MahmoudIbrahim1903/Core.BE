using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.Domain.AggregatesModel
{
    public class MailTemplate : Entity, IAggregateRoot
    {
        public string MailType { get; set; }
        public string TemplateNameEn { get; set; }
        public string TemplateNameAr { get; set; }
        public MailTemplate()
        {

        }
    }
}
