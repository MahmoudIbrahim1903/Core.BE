using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.SMS.Infractructure.Model
{
    public interface ISmsMessageRepository
    {
        SmsMessage SaveSmsMessage(SmsMessage smsMessage);
        MessageUser AddMessageUser(int messageId, int userId, string createdBy);
        MessageStatus AddMessageStatus(int messageUserId, int statusCode, string StatusMessage, string createdBy);
        void SaveChanges();
    }
}
