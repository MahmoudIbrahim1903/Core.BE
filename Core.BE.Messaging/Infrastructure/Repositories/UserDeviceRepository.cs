using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.Repositories
{
    public class UserDeviceRepository : BaseRepository<UserDevice, MessagingContext>, IUserDeviceRepository
    {
        public UserDeviceRepository(MessagingContext context) : base(context)
        {
        }
        public UserDevice GetUserDeviceByCode(string code)
        {
            return _context.Set<UserDevice>().FirstOrDefault(n => n.Code == code);
        }


        public UserDevice GetUserDeviceByUserId(int userId)
        {
            return _context.Set<UserDevice>().FirstOrDefault(n => n.Id == userId);
        }
    }
}
