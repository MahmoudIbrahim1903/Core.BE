using Emeint.Core.BE.Chatting.Domain.Configurations;
using Emeint.Core.BE.Chatting.Infractructure.IRepositories;
using Emeint.Core.BE.Chatting.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Services.Concretes
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfigurationManager _configurationManager;

        public UserService(IUserRepository userRepository, IConfigurationManager configurationManager)
        {
            _userRepository = userRepository;
            _configurationManager = configurationManager;
        }
        public List<string> GetUsersTypes(bool excludeAdmin)
        {
            var users = _userRepository.GetUsers();

            if (excludeAdmin)
                users = users.Where(u => u.Id != _configurationManager.GetChattingAdminId()).ToList();

            return users.Select(u => u.Type).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList();
        }
    }
}
