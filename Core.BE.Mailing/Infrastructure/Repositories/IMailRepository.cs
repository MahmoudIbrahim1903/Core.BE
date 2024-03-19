using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Mailing.Domain.AggregatesModel;
using Emeint.Core.BE.Mailing.Domain.Enums;
using System.Linq;

namespace Emeint.Core.BE.Mailing.Infrastructure.Repositories
{
    public interface IMailRepository : IRepository<Mail>
    {
        IQueryable<Mail> GetMailsByStatus(Status status);
    }
}
