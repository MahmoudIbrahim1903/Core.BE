using Emeint.Core.BE.Enums;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class InvalidParametersException : BaseException
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }


        public InvalidParametersException()
        {
            ErrorCode = (int)ErrorCodes.InternalServerError;
            ErrorMessage = "Invalid Parameter";
        }
    }
}