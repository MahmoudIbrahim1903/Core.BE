using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.DevicePlatformAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.PlatformAggregate
{
    public interface IDevicePlatformRepository:IRepository<DevicePlatform>
    {
        DevicePlatform GetPlatformByCode(string code);
    }
}
