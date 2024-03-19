using Emeint.Core.BE.Identity.Domain.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Managers
{
    public class ExternalTokenManager : IExternalTokenManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ExternalTokenManager(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> GetExternalProviderToken(string userName)
        {
            var user = _userManager.FindByNameAsync(userName).Result;
            return user.ExternalProviderToken;
        }
    }
}
