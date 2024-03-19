using Emeint.Core.BE.Chatting.Infractructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Infractructure.IRepositories
{
    public interface IUserRepository
    {
        bool IsRegistered(string userId);
        void RegisterUser(string id, string name, string type, string imageUrl);
        void UpdateRegisteredUser(string userId, string name, string type, string imageUrl);
        List<User> GetUsers();

    }
}
