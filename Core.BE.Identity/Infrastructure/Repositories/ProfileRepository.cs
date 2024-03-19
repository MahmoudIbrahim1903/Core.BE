using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public class ProfileRepository : BaseRepository<Profile, ApplicationDbContext>,
         IProfileRepository
    {
        public ProfileRepository(ApplicationDbContext context) : base(context)
        {
        }
        public override Profile Add(Profile entity)
        {
            return base.Add(entity);
        }
        public ApplicationUser GetUserById(string userId)
        {
            return _context.Set<ApplicationUser>().Where(n => n.Id == userId).FirstOrDefault();
        }
        public List<ApplicationUser> GetAllUsers()
        {
            return _context.Set<ApplicationUser>().ToList();
        }

    }
}
