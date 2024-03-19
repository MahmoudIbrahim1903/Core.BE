using Emeint.Core.BE.Domain.Enums;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class MissingParameterException : BusinessException
    {
        public MissingParameterException(string parameterName) : this(parameterName, null)
        {
        }

        public MissingParameterException(string parameterName, string moreDetails)
        {
            Code = (int)ErrorCodes.MissingParameter;
            Resourcekey = ErrorCodes.MissingParameter.ToString();
            MessageParameters.Add(parameterName);
            MessageEn = string.Format("Missing parameter: " + parameterName);
            MessageAr = string.Format("الرجاء التاكد من ارسال قيمة ل: " + parameterName);
            MoreDetails = moreDetails;
        }
    }
}