using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.AggregatesModel.ApplicationAggregate
{
    public interface IApplicationRepository:IRepository<ClientApplicationVersion>
    {
        ClientApplicationVersion GetClientApplicationVersionByCode(string code);
    }
}
