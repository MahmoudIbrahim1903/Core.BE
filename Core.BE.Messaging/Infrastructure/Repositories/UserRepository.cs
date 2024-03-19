using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User, MessagingContext>, IUserRepository
    {
        public UserRepository(MessagingContext context) : base(context)
        {
        }

        public List<User> GetUserByToken(string pushToken)
        {
            return _context.Set<User>().Include(u => u.UserDevice).Where(n => n.PushNotificationToken == pushToken).ToList();
        }

        public User GetUserByUserRegisterId(string registerId)
        {
            return _context.Set<User>().Include(u => u.UserDevice).FirstOrDefault(n => n.ApplicationUserId == registerId);
        }

        public List<User> GetUsers(AdminGetUsersRequestViewModel criteria)
        {
            var pageSize = criteria?.Pagination?.PageSize ?? 10;
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var pageNumber = criteria?.Pagination?.PageNumber != null ? (int)((criteria?.Pagination?.PageNumber - 1) * criteria?.Pagination?.PageSize) : 0;

            if (criteria.Pagination != null && criteria.Pagination.PageSize != null && criteria.Pagination.PageNumber != null && criteria.Pagination.PageSize > 0)
            {
                return _context.Set<User>().Skip(pageNumber).Take(pageSize).ToList();
            }
            else
            {
                return _context.Set<User>().ToList();
            }
        }
    }
}
