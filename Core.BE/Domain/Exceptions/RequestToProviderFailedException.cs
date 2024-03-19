using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Emeint.Core.BE.Domain.Exceptions
{
    public class RequestToProviderFailedException : BusinessException
    {
        public HttpStatusCode StatusCode { get; set; }
        public string OriginalContent { get; set; }
        public RequestToProviderFailedException(HttpStatusCode httpStatus, string errorMessage)
        {
            StatusCode = httpStatus;
            OriginalContent = errorMessage;

            Code = (int)ErrorCodes.RequestToProviderFailedException;
            MessageEn = "Request failed with status: " + httpStatus.ToString() + " and error message: " + errorMessage;
            MessageAr = "Request failed with status: " + httpStatus.ToString() + " and error message: " + errorMessage;
        }
    }
}