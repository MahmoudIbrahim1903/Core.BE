using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.Model
{
    public class MessageUser : Entity
    {
        public MessageUser()
        {

        }
        public MessageUser(string createdBy)
        {
            Code = $"MU-{new Random().Next(1, 10000)}{DateTime.UtcNow.ToString("yyMMddhhmmss")}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;
        }

        public int UserId { set; get; }
        public int SmsMessageId { set; get; }

        public virtual User User { set; get; }
        public virtual SmsMessage SmsMessage { set; get; }
    }
}
