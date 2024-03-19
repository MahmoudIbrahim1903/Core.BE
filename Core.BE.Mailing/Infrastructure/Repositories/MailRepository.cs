using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Mailing.Domain.AggregatesModel;
using Emeint.Core.BE.Mailing.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.Infrastructure.Repositories
{
    public class MailRepository : BaseRepository<Mail, MailingContext>, IMailRepository
    {
        public MailRepository(MailingContext context) : base(context)
        {
        }
        public IQueryable<Mail> GetMailsByStatus(Status status)
        {
            // TODO: Handle disposed context
            return _context.Set<Mail>().Where(n => n.Status == status);
        }
    }
}
