using Emeint.Core.BE.Domain.Enums;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidParameterException : BusinessException
    {
        //private string _message;
        //public override string Message
        //{
        //    get
        //    {
        //        return _message;
        //    }
        //}

        public InvalidParameterException(string parameterName, string parameterValue) : this(parameterName, parameterValue, null)
        {
        }

        public InvalidParameterException(string parameterName, string parameterValue, string moreDetails)
        {
            Code = (int)ErrorCodes.InvalidParameter;
            Resourcekey = ErrorCodes.InvalidParameter.ToString();
            MessageParameters.Add(parameterValue);
            MessageParameters.Add(parameterName);
            MessageEn = string.Format("Invalid value: " + parameterValue + " for parameter: " + parameterName);
            MessageAr = string.Format("قيمه غير سليمة: " + parameterValue + " لهذا المتغير: " + parameterName);
            MoreDetails = moreDetails;
        }

        //public InvalidParameterException(string parameterName, int errorDetails)
        //{
            //this.Response.error_code = (int)ErrorCodes.MissingParameter;
            //this.Response.error_msg = string.Format(Resources.Resources.user_error_invalid_parameter,
            //                                        parameterName);
            //if (errorDetails > 0)
            //{
            //    this.Response.error_details = errorDetails;
            //}

            //_message = parameterName;
        //}
    }
}