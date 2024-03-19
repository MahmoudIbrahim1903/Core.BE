using System;

namespace Emeint.Core.BE.API.Infrastructure.Middlewares.LoggingMiddlewares
{
    public class ClientRequestLoggingMiddleware
    {
        public int Id { get; set; }
        public string RequestClientReferenceNumber { get; set; }
        public string HttpMethod { get; set; }
        public string Path { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }
        public DateTime Time { get; set; }


    }
}
