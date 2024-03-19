using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate
{
    public interface IUserDeviceRepository : IRepository<UserDevice>
    {
        UserDevice GetUserDeviceByCode(string code); 
        UserDevice GetUserDeviceByUserId(int userId);
    }
}
