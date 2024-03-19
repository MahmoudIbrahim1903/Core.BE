using Emeint.Core.BE.Domain.Enums;
using System.Collections.Generic;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidUserInputException : BusinessException
    {
        public InvalidUserInputException(string resourceKey, List<string> messageParameters = null)
        {
            Code = (int)ErrorCodes.InvalidUserInput;
            Resourcekey = resourceKey;
            if (messageParameters != null && messageParameters.Count > 0)
                MessageParameters.AddRange(messageParameters);
        }
        public InvalidUserInputException(string messageEn, string messageAr)
        {
            Code = (int)ErrorCodes.InvalidUserInput;
            MessageEn = messageEn;
            MessageAr = messageAr;
        }

    }
}
