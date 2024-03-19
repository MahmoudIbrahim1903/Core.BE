using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.Model
{
    public interface IUserRepository
    {
        User GetUserByPhoneNumber(string phoneNumber);
        User AddUser(string phoneNumber, string createdBy);

    }
}
