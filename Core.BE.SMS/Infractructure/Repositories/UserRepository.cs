using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.SMS.Infractructure.Data;
using Emeint.Core.BE.SMS.Infractructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.Repositories
{
    public class UserRepository : BaseRepository<User, SmsDbContext>, IUserRepository
    {
        public UserRepository(SmsDbContext smsDbContext) : base(smsDbContext)
        {

        }

        public User GetUserByPhoneNumber(string phoneNumber)
        {
            return _context.Users.FirstOrDefault(u => u.PhoneNumber.Contains(phoneNumber.Trim()));
        }

        public User AddUser(string phoneNumber, string createdBy)
        {
            var user = _context.Users.Add(new User(createdBy)
            {
                PhoneNumber = phoneNumber
            }).Entity;
            _context.SaveChanges();
            return user;
        }


    }
}
