using Emeint.Core.BE.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Exceptions
{
    public class MessagingDomainException: BaseException
    {
        public MessagingDomainException()
        { }

        public MessagingDomainException(string message)
            : base(message)
        { }

        public MessagingDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
