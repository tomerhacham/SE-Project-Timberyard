using System;
using System.Collections.Generic;

namespace ETL.Utils
{
    public enum LogEntry
    {
        //MetaData
        Component
    }
    public interface ILogger
    {
        //Info
        void Info(string message, Dictionary<LogEntry, string> extraInfo = null, string methodName = default(string));

        //Warning
        void Warning(string message, Exception ex = null, Dictionary<LogEntry, string> extraInfo = null);
    }
}
