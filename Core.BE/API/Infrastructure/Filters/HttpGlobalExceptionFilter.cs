//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Logging;
//using System.Net;
//using Emeint.Core.BE.Domain.Exceptions;
//using Emeint.Core.BE.Domain.Enums;
//using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
//using Emeint.Core.BE.API.Infrastructure.Services;
//using System;

//namespace Emeint.Core.BE.API.Infrastructure.Filters
//{

//    public class HttpGlobalExceptionFilter : IExceptionFilter
//    {
//        private readonly IHostingEnvironment _hostingEnvironment;
//        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
//        //private readonly IdentityService identityService;

//        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
//        {
//            this._hostingEnvironment = env;
//            this._logger = logger;
//        }

//        public void OnException(ExceptionContext context)
//        {
//            _logger.LogError("-----ERROR-----");

//            _logger.LogError("Stack trace:");
//            _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

//            bool handled = false;

//            //.. Handling Status Code ..//
//            if (context.Exception is AggregateException)
//            {
//                var aggregateException = context.Exception as AggregateException;
//                aggregateException.Handle((x) =>
//                {
//                    if (x is UnauthorizedAccessException)
//                    {
//                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                        handled = true;
//                        return true;
//                    }
//                    else if (x is ForbiddenException)
//                    {
//                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
//                        handled = true;
//                        return true;
//                    }
//                    else
//                    {
//                        handled = false;
//                        return true;
//                    }
//                    return false;
//                });
//            }

//            if (!handled)
//            {
//                if (context.Exception is UnauthorizedAccessException)
//                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                else if (context.Exception is ForbiddenException)
//                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
//                else
//                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

//                BaseResponse errorResponse = new BaseResponse();
//                if (context.Exception is BaseException)
//                {
//                    BaseException baseException = context.Exception as BaseException;

//                    errorResponse.ErrorCode = baseException.Code;
//                    if (context.HttpContext.Request.Headers["Language"] == "ar")
//                    {
//                        errorResponse.ErrorMsg = baseException.MessageAr;
//                    }
//                    else if (context.HttpContext.Request.Headers["Language"] == "en")
//                    {
//                        errorResponse.ErrorMsg = baseException.MessageEn;
//                    }
//                    else
//                    {
//                        errorResponse.ErrorMsg = baseException.MessageEn;
//                    }
//                    errorResponse.ErrorDetails = baseException.MoreDetails;
//                }
//                else
//                {
//                    var exType = context.Exception.GetType().BaseType;
//                    if (exType.Name == "BaseException")
//                    {
//                        errorResponse.ErrorCode = (int)exType.GetProperty("Code").GetValue(context.Exception);
//                        if (context.HttpContext.Request.Headers["Language"] == "ar")
//                        {
//                            errorResponse.ErrorMsg = exType.GetProperty("MessageAr").GetValue(context.Exception)?.ToString();
//                        }
//                        else if (context.HttpContext.Request.Headers["Language"] == "en")
//                        {
//                            errorResponse.ErrorMsg = exType.GetProperty("MessageEn").GetValue(context.Exception)?.ToString();
//                        }
//                        else
//                        {
//                            errorResponse.ErrorMsg = exType.GetProperty("MessageEn").GetValue(context.Exception)?.ToString();
//                        }
//                        errorResponse.ErrorDetails = exType.GetProperty("MoreDetails").GetValue(context.Exception)?.ToString();
//                    }
//                    else
//                    {
//                        errorResponse.ErrorCode = (int)ErrorCodes.InternalServerError;
//                        errorResponse.ErrorMsg = context.Exception.Message;

//                        if (context.Exception.InnerException != null)
//                            errorResponse.ErrorDetails = context.Exception.InnerException.Message;
//                    }
//                }

//                _logger.LogError("Response error code: " + errorResponse.ErrorCode);
//                _logger.LogError("Response error message: " + errorResponse.ErrorMsg);
//                if (errorResponse.ErrorDetails != null)
//                    _logger.LogError("Response error details: " + errorResponse.ErrorDetails);

//                context.Result = new ObjectResult(errorResponse);
//            }

//            _logger.LogError("-----END ERROR-----");

//            context.ExceptionHandled = true;

//            //if (context.Exception.GetType() == typeof(BaseException))
//            //{
//            //    //var json = new JsonErrorResponse
//            //    //{
//            //    //    Messages = new[] { context.Exception.Message }
//            //    //};

//            //    // Result asigned to a result object but in destiny the response is empty. This is a known bug of .net core 1.1
//            //    //It will be fixed in .net core 1.1.2. See https://github.com/aspnet/Mvc/issues/5594 for more information
//            //    context.Result = new BadRequestObjectResult(json);
//            //    //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//            //}
//            //else
//            //{
//            //    var json = new JsonErrorResponse
//            //    {
//            //        Messages = new[] { "An error occur.Try it again." }
//            //    };

//            //    if (env.IsDevelopment())
//            //    {
//            //        json.DeveloperMessage = context.Exception;
//            //    }

//            //    // Result asigned to a result object but in destiny the response is empty. This is a known bug of .net core 1.1
//            //    // It will be fixed in .net core 1.1.2. See https://github.com/aspnet/Mvc/issues/5594 for more information
//            //    //context.Result = new InternalServerErrorObjectResult(json);
//            //    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//            //}

//        }

//        //private class JsonErrorResponse
//        //{
//        //    public string[] Messages { get; set; }

//        //    public object DeveloperMessage { get; set; }
//        //}
//    }
//}