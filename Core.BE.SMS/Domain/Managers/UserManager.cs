using Emeint.Core.BE.SMS.Infractructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Domain.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User AddUser(string phoneNumber,string createdBy)
        {
            var user = _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (user == null)
                user = _userRepository.AddUser(phoneNumber, createdBy);
            return user;
        }
    }
}
