using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.ApplicationAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Infrastructure.Repositories
{
    public class ApplicationRepository : BaseRepository<ClientApplicationVersion, MessagingContext>, IApplicationRepository
    {
        public ApplicationRepository(MessagingContext context) : base(context)
        {
        }
        public ClientApplicationVersion GetClientApplicationVersionByCode(string code)
        {
            throw new NotImplementedException();
        }
    }
}
