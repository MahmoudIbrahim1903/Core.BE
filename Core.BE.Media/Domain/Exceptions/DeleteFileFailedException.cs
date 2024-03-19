using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Enums;

namespace Emeint.Core.BE.Media.Domain.Exceptions
{
    public class DeleteFileFailedException : RuntimeException
    {
        //private string _message;
        //public override string Message
        //{
        //    get
        //    {
        //        return _message;
        //    }
        //}

        public DeleteFileFailedException(string fileName) : this(fileName, null)
        {
        }

        public DeleteFileFailedException(string fileName, string moreDetails)
        {
            Code = (int)MediaErrorCodes.DeleteFileFailed;
            MessageEn = string.Format("Failed to delete: " + fileName);
            MessageAr = string.Format("فشلت عملية الحذف: " + fileName);
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