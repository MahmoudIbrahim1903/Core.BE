using Emeint.Core.BE.API.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.API.Infrastructure.Middlewares.LoggingMiddlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        //private readonly TContext _context;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            //_context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Invoke(HttpContext httpContext, IIdentityService identityService)
        {
            var injectedRequestStream = new MemoryStream();
            StringValues reqId;
            StringValues headerValues;
            string bodyAsText;

            var userId = identityService?.UserId;
            if (!string.IsNullOrEmpty(userId))
                _logger.LogInformation($"---UserId: {userId}---");
            //string headers = string.Empty;

            try
            {
                var requestLog = $"REQUEST: {httpContext.Request.Method} {httpContext.Request.Path}{Environment.NewLine}";

                httpContext.Request.Headers.TryGetValue("Request-Client-Reference-Number", out reqId);

                ///Uncomment this line if you need the hearders logged as json object format. 
                ///Comment the next foreach.
                //headerValues = Newtonsoft.Json.JsonConvert.SerializeObject(httpContext.Request.Headers);

                ///Uncomment this line if you need headers logged as string format
                ///separated with new lines.
                ///Comment the line of code above.
                foreach (var key in httpContext.Request.Headers.Keys)
                {
                    headerValues += $"{key}: {httpContext.Request.Headers[key]} {Environment.NewLine}";
                }
                requestLog += $"HEADERS:{Environment.NewLine}{headerValues}";
                requestLog += $"END HEADERS{Environment.NewLine}";
                if (httpContext.Request.ContentType != null && !httpContext.Request.ContentType.Contains("multipart/form-data"))
                {
                    using (var bodyReader = new StreamReader(httpContext.Request.Body))
                    {
                        bodyAsText = bodyReader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(bodyAsText) == false)
                        {
                            requestLog += $"BODY:{Environment.NewLine}{bodyAsText}";
                            requestLog += $"{Environment.NewLine}END BODY{Environment.NewLine}";
                        }

                        var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
                        injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                        injectedRequestStream.Seek(0, SeekOrigin.Begin);
                        httpContext.Request.Body = injectedRequestStream;
                    }
                }
                // Stop logging in database
                //var request = new ClientRequestLoggingMiddleware
                //{
                //    RequestId = reqId,
                //    HttpMethod = httpContext.Request.Method,
                //    Path = httpContext.Request.Path + httpContext.Request.QueryString,
                //    Headers = headerValues,
                //    Body = bodyAsText,
                //    Time = DateTime.UtcNow
                //};
                //_context.Add(request);

                _logger.LogInformation(requestLog);
                //_logger.LogInformation(requestLog);
                //await _context.SaveChangesAsync();
                await _next.Invoke(httpContext);
            }
            finally
            {
                injectedRequestStream.Dispose();
            }
        }
    }
}