//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Reflection;
//using System.Text;

//namespace Emeint.Core.BE.API.Infrastructure
//{
//    public class CurrentLocationLogger
//    {
//        ILogger _logger;

//        public CurrentLocationLogger(ILogger logger)
//        {
//            _logger = logger;
//        }

//        public void LogInformation(string message)
//        {
//            MethodBase mb = GetCallingMethod();
//            Type t = mb.DeclaringType;
//            LogEventInfo logEvent = new LogEventInfo(LogLevel.Debug, t.Name, null, "{0}", new object[] message, null);

//            StackTrace stackTrace = new StackTrace();
//            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();

//            _logger.LogInformation($"{methodBase.Name}: {message}");
//        }
//    }
//}