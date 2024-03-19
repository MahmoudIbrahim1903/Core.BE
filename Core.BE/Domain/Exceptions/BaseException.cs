using System;
using System.Collections.Generic;
using System.Reflection;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class BaseException : Exception
    {
        public string Message { get; set; }
        public string Resourcekey { get; set; }
        public List<string> MessageParameters { get; set; }
        public string MessageAr { get; set; }
        public string MessageEn { get; set; }
        public int Code { get; set; }
        public string MoreDetails { get; set; }

        public BaseException()
        {
            MessageParameters = new List<string>();
        }

        public BaseException(string message)
            : base(message)
        {
            MessageParameters = new List<string>();
        }

        public BaseException(string message, Exception innerException)
            : base(message, innerException)
        {
            MessageParameters = new List<string>();
        }

        //#region Fields

        //Response<T> _response = new Response<T>((int)ErrorCodes.InternalServerError, string.Empty);
        //#endregion

        //#region Properties

        //public response Response
        //{
        //    get { return _response; }
        //    set { _response = value; }
        //}

        //#endregion


        //#region Constructors

        //public BaseException()
        //{
        //}
        //public BaseException(ErrorCodes code, string message)
        //{
        //    _response = new response { error_code = (int)code, error_msg = message };
        //}
        //public BaseException(response response)
        //{
        //    _response = response;
        //}

        //public BaseException(string message)
        //    : base(message)
        //{
        //    _response.error_msg = message;
        //}

        //public BaseException(Exception innerException)
        //    : this(string.Empty, innerException)
        //{ }

        //public BaseException(string message, Exception innerException)
        //    : base(message, innerException)
        //{
        //    _response.error_msg = message;
        //}

        //#endregion

    }
}
