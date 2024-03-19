using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByUserRegisterId(string registerId);
        List<User> GetUsers(AdminGetUsersRequestViewModel criteria);
        List<User> GetUserByToken(string pushToken);
    }
}
