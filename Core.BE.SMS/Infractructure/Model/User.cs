using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.Model
{
    public class User : Entity
    {

        public User()
        {

        }
        public User(string createdBy)
        {
            Code = $"Ur-{new Random().Next(1, 10000)}{DateTime.UtcNow.ToString("yyMMddhhmmss")}";
            CreatedBy = createdBy;
            CreationDate = DateTime.UtcNow;
        }


        public string PhoneNumber { set; get; }
    }
}
