using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Mailing.Domain.AggregatesModel;

namespace Emeint.Core.BE.Mailing.Infrastructure.Repositories
{
    public interface IMailTemplateRepository : IRepository<MailTemplate>
    {
        MailTemplate GetMailTemplateByMailType(string mailType);
    }
}
