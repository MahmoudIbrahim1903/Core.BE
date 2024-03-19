using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.Enums;
using System.Net;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Emeint.Core.BE.Utilities;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.API.Infrastructure.Middlewares
{
    public class ExceptionCatchMiddleware
    {
        private readonly RequestDelegate _delegate;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        private class BaseResponse
        {
            [DataMember]
            public int error_code { get; set; }

            [DataMember]
            public string error_msg { get; set; }

            [DataMember]
            public string error_details { get; set; }

            [DataMember]
            public Expiration expiration { get; set; }

            [DataMember]
            public Persistence persistence { get; set; }

            [DataMember]
            public int total_seconds { get; set; }
        }
        public class Expiration
        {
            public Expiration()
            {

            }

            [DataMember]
            public bool is_allowed { get; set; }

            [DataMember]
            public int duration { get; set; } // In seconds

            [DataMember]
            public int method { get; set; }

            [DataMember]
            public int mode { get; set; }

            [DataMember]
            public bool is_session_expiry { get; set; }

            public enum ExpiryDuration
            {
                //time in seconds
                NoExpiry = 0,
                QuarterHour = 15 * 60,
                HalfHour = 30 * 60,
                OneHour = 60 * 60,
                OneDay = 24 * 60 * 60
            }
        }
        public class Persistence
        {
            public Persistence()
                : this((int)ScopeLevel.App, false)
            {
            }

            public Persistence(int scope, bool isEncrypted)
            {
                this.scope = scope;
                this.is_encrypted = isEncrypted;
            }

            [DataMember]
            public int scope { get; set; }

            [DataMember]
            public bool is_encrypted { get; set; }

            public enum ScopeLevel
            {
                App = 0,
                User = 1
            }
        }


        public ExceptionCatchMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionCatchMiddleware> logger, IConfiguration configuration)
        {
            _delegate = requestDelegate;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            // TODO: Refactor
            try
            {
                await _delegate(context);
            }
            //catch (ReflectionTypeLoadException e)
            //{
            //    foreach (Exception ex in e.LoaderExceptions)
            //    {
            //        _logger.LogCritical(ex.Message + Environment.NewLine + ex.StackTrace);
            //    }
            //}
            catch (Exception e)
            {
                _logger.LogCritical(e.Message + Environment.NewLine + e.StackTrace);

                ////////////////////////////////////////////////////

                _logger.LogError("-----ERROR-----");

                _logger.LogError("Stack trace:");
                _logger.LogError(new EventId(e.HResult), e, e.Message);

                bool handled = false;

                //.. Handling Status Code ..//
                if (e is AggregateException)
                {
                    var aggregateException = e as AggregateException;
                    aggregateException.Handle((x) =>
                    {
                        if (x is UnauthorizedAccessException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            handled = true;
                            return true;
                        }
                        else if (x is ForbiddenException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            handled = true;
                            return true;
                        }
                        else
                        {
                            handled = false;
                            return true;
                        }
                        return false;
                    });
                }

                if (!handled)
                {
                    if (e is UnauthorizedAccessException)
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    else if (e is ForbiddenException)
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    else
                        context.Response.StatusCode = (int)HttpStatusCode.OK;

                    BaseResponse errorResponse = new BaseResponse();
                    if (e is BaseException)
                    {
                        BaseException baseException = e as BaseException;
                        errorResponse.error_code = baseException.Code;
                        string arResourceMessage = string.Empty, swResourceMessage = string.Empty, enResourceMessage = string.Empty;
                        if (!string.IsNullOrEmpty(baseException.Resourcekey))
                        {
                            arResourceMessage = LocalizationUtility.GetLocalizedResourceText(baseException.Resourcekey, baseException.MessageParameters, Language.ar);
                            swResourceMessage = LocalizationUtility.GetLocalizedResourceText(baseException.Resourcekey, baseException.MessageParameters, Language.sw);
                            enResourceMessage = LocalizationUtility.GetLocalizedResourceText(baseException.Resourcekey, baseException.MessageParameters, Language.en);
                        }
                        if (context.Request.Headers["Language"] == "ar")
                        {
                            if (!string.IsNullOrEmpty(arResourceMessage))
                            {
                                baseException.Message = arResourceMessage;
                                errorResponse.error_msg = baseException.Message;
                            }
                            else
                             errorResponse.error_msg = baseException.MessageAr;
                        }
                        else if (context.Request.Headers["Language"] == "sw")
                        {
                            if (!string.IsNullOrEmpty(swResourceMessage))
                            {
                                baseException.Message = swResourceMessage;
                                errorResponse.error_msg = baseException.Message;
                            }
                            else
                                errorResponse.error_msg = baseException.MessageEn ?? enResourceMessage;
                        }
                        else if (context.Request.Headers["Language"] == "en")
                        {
                            if (!string.IsNullOrEmpty(enResourceMessage))
                            {
                                baseException.Message = enResourceMessage;
                                errorResponse.error_msg = baseException.Message;
                            }
                            else
                                errorResponse.error_msg = baseException.MessageEn;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(enResourceMessage))
                            {
                                baseException.Message = enResourceMessage;
                                errorResponse.error_msg = baseException.Message;
                            }
                            else
                                errorResponse.error_msg = baseException.MessageEn;
                        }
                        errorResponse.error_details = baseException.MoreDetails;
                    }
                    else
                    {
                        var exType = e.GetType().BaseType;
                        if (exType.Name == "BaseException")
                        {
                            errorResponse.error_code = (int)exType.GetProperty("Code").GetValue(e);
                            if (!string.IsNullOrEmpty(exType.GetProperty("Message").GetValue(e)?.ToString()))
                            {
                                errorResponse.error_msg = exType.GetProperty("Message").GetValue(e)?.ToString();
                            }
                            else if (context.Request.Headers["Language"] == "ar")
                            {
                                errorResponse.error_msg = exType.GetProperty("MessageAr").GetValue(e)?.ToString();
                            }
                            else if (context.Request.Headers["Language"] == "en")
                            {
                                errorResponse.error_msg = exType.GetProperty("MessageEn").GetValue(e)?.ToString();
                            }
                            else
                            {
                                errorResponse.error_msg = exType.GetProperty("MessageEn").GetValue(e)?.ToString();
                            }
                            errorResponse.error_details = exType.GetProperty("MoreDetails").GetValue(e)?.ToString();
                        }
                        else
                        {
                            errorResponse.error_code = (int)ErrorCodes.InternalServerError;
                            errorResponse.error_msg = e.Message;

                            if (e.InnerException != null)
                                errorResponse.error_details = e.InnerException.Message;
                        }
                    }

                    _logger.LogError("Response error code: " + errorResponse.error_code);
                    _logger.LogError("Response error message: " + errorResponse.error_msg);
                    if (errorResponse.error_details != null)
                        _logger.LogError("Response error details: " + errorResponse.error_details);

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse)); // TODO: snake_case
                }

                _logger.LogError("-----END ERROR-----");


            }
        }
    }
}