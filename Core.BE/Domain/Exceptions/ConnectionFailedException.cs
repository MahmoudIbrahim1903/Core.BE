using Emeint.Core.BE.Domain.Enums;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public  class ConnectionFailedException : BaseException
    {
        //public static string MessageEn { get; set; }
        //public static string MessageAr { get; set; }
        public ConnectionFailedException()
        {
            Code = (int)ErrorCodes.ConnectionToProviderFailed;
            MessageEn = "Connection failed!";
            MessageAr = "تعذر الاتصال بالخادم";
        }
    }
}