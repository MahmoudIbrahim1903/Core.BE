using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    public class PushMessageResponse
    {
        public List<string> SuccessfulSentIds { get; set; }
        public List<string> FailureSentIds { get; set; }
        public bool IsAllSucceeded { get; set; }
        public List<KeyValuePair<string, string>> FailedUsersIdsExceptionsMessages { get; set; }
        public PushMessageResponse()
        {
            SuccessfulSentIds = new List<string>();
            FailureSentIds = new List<string>();
            FailedUsersIdsExceptionsMessages = new List<KeyValuePair<string, string>>();
        }
    }
}
