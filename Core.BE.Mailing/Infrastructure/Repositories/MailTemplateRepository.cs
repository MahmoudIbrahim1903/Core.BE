using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Mailing.Domain.AggregatesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.Infrastructure.Repositories
{
    public class MailTemplateRepository : BaseRepository<MailTemplate, MailingContext>, IMailTemplateRepository
    {
        public MailTemplateRepository(MailingContext context) : base(context)
        {
        }
        public MailTemplate GetMailTemplateByMailType(string mailType)
        {
            return _context.Set<MailTemplate>().Where(n => n.MailType == mailType).FirstOrDefault();
        }
    }
}
