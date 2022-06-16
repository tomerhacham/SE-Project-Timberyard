using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WebService.Utils
{
    public class Logger : ILogger
    {
        private readonly NLog.Logger? _logger = null;

        public Logger(string loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }
        private string GetMethodName()
        {
            try
            {
                var stackTrace = new StackTrace();
                var methodName = $"{stackTrace.GetFrame(2).GetMethod().ReflectedType}.{ stackTrace.GetFrame(2).GetMethod().Name}";
                var index = methodName.IndexOf("+");
                if (index > 0)
                {
                    var regex = new Regex(@"<(\w+)>.*");
                    var asyncMethodName = regex.Match(methodName).Groups[1].Value;

                    var asyncFileName = methodName.Substring(0, index);
                    methodName = asyncFileName + '.' + asyncMethodName;
                }

                return methodName;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "{methodName} {message} {threadId}",
                    "", "Failed to extract method name", Thread.CurrentThread.ManagedThreadId);

                return string.Empty;
            }
        }
        private void WriteCustomLog(LogLevel logLevel, string methodName, string message, Dictionary<LogEntry, string>? userExtraInfo = null, Exception? ex = null)
        {
            try
            {
                message = AppendData(message, userExtraInfo);
                Log(logLevel, methodName, message, ex);
            }
            catch (Exception exception)
            {
                Log(LogLevel.Error, methodName, "Cannot write to log", exception);
            }
        }
        private void Log(LogLevel logLevel, string methodName, string message, Exception ex)
        {
            var requestid = MappedDiagnosticsLogicalContext.Get("requestid");
            _logger.Log(logLevel, ex, "{methodName} {message} {threadId} {requestid}",
                   methodName, message, Thread.CurrentThread.ManagedThreadId, requestid);
        }
        public static string AppendData(string message, Dictionary<LogEntry, string> extraInfo)
        {
            //add extra info
            if (extraInfo != null)
            {
                StringBuilder str = new StringBuilder();
                foreach (var info in extraInfo)
                {
                    //convert all keys value in to grock format: key=value| key=value| 
                    str.Append($" |{info.Key}={info.Value}");
                }

                //add new keys after message key
                message += str.ToString();

            }
            //remove '[' ']'           
            message = message.Replace('[', '{');
            return message.Replace(']', '}');
        }
        //Info       
        public void Info(string message, Dictionary<LogEntry, string>? extraInfo = null, string? methodName = null)
        {
            methodName = string.IsNullOrEmpty(methodName) ? GetMethodName() : methodName;
            WriteCustomLog(LogLevel.Info, methodName, message, extraInfo);
        }
        //Warning
        public void Warning(string message, Exception? ex = null, Dictionary<LogEntry, string>? extraInfo = null)
        {
            var methodName = GetMethodName();
            WriteCustomLog(LogLevel.Warn, methodName, message, extraInfo, ex);
        }
    }
}
