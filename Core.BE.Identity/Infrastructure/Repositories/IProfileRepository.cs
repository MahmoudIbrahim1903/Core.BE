using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Identity.Domain.Model;
using System;
using System.Collections.Generic;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public interface IProfileRepository : IRepository<Profile>
    {
        ApplicationUser GetUserById(string userId);
        List<ApplicationUser> GetAllUsers();


    }
}