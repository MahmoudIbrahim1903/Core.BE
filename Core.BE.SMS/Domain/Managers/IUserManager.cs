using Emeint.Core.BE.SMS.Infractructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.Managers
{
    public interface IUserManager
    {
        User AddUser(string phoneNumber, string createdBy);

    }
}
