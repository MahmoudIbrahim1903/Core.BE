using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.SMS.Infractructure.Data;
using Emeint.Core.BE.SMS.Infractructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.Repositories
{
    public class SmsMessageRepository : BaseRepository<SmsMessage, SmsDbContext>, ISmsMessageRepository
    {
        public SmsMessageRepository(SmsDbContext smsDbContext) : base(smsDbContext)
        {

        }

        public SmsMessage SaveSmsMessage(SmsMessage smsMessage)
        {
            var messge = _context.SmsMessages.Add(smsMessage).Entity;
            _context.SaveChanges();
            return messge;
        }

        public MessageUser AddMessageUser(int messageId, int userId, string createdBy)
        {
            var messageUser = _context.MessageUsers.Add(new MessageUser(createdBy)
            {
                UserId = userId,
                SmsMessageId = messageId,

            }).Entity;
            _context.SaveChanges();
            return messageUser;
        }

        public MessageStatus AddMessageStatus(int messageUserId, int statusCode, string StatusMessage, string createdBy)
        {
            var messageStatus = _context.MessageStatuses.Add(new MessageStatus(createdBy)
            {
                MessageUserId = messageUserId,
                StatusCode = statusCode,
                StatusMessage = StatusMessage

            }).Entity;
            _context.SaveChanges();
            return messageStatus;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
