using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.SMS.Infractructure.Data;
using Emeint.Core.BE.SMS.Infractructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.Repositories
{
    public class MessageTemplateRepository : BaseRepository<MessageTemplate, SmsDbContext>, IMessageTemplateRepository
    {
        public MessageTemplateRepository(SmsDbContext context) : base(context)
        {

        }

        public MessageTemplate GetTemplateByCode(string templateCode)
        {
            return _context.MessageTemplates
                .FirstOrDefault(t => t.Code.Trim().ToLower() == templateCode.Trim().ToLower());
        }
    }
}
