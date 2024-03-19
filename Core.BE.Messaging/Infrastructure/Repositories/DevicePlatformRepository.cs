using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.PlatformAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.Repositories
{
    public class DevicePlatformRepository : BaseRepository<DevicePlatform, MessagingContext>, IDevicePlatformRepository
    {
        public DevicePlatformRepository(MessagingContext context) : base(context)
        {
        }
        public DevicePlatform GetPlatformByCode(string code)
        {
            throw new NotImplementedException();
        }
    }
}
