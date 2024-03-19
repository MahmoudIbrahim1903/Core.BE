using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.Model
{
    public class MessageStatus : Entity
    {
        public MessageStatus()
        {

        }
        public MessageStatus(string createdBy)
        {

            Code = $"MS-{new Random().Next(1, 10000)}{DateTime.UtcNow.ToString("yyMMddhhmmss")}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;
        }

        public int StatusCode { set; get; }
        public string StatusMessage { set; get; }

        public int MessageUserId { set; get; }
        public virtual MessageUser MessageUser { set; get; }

    }
}
