using System;
using System.Diagnostics;

namespace DocumentSigner.Helpers
{
    public abstract class LogHelper
    {
        public const string SourceName = "DocumentSigner";
        public const string LogName = "Application";

        public static void WriteLog(string message, EventLogEntryType eventLogEntryType)
        {
            EventLog.WriteEntry(SourceName, message, eventLogEntryType);
        }

        public static void WriteLog(string description, Exception exception, EventLogEntryType eventLogEntryType)
        {
            var message = description
                + Environment.NewLine + exception.Message
                + Environment.NewLine + exception.StackTrace;

            while (exception.InnerException != null)
            {
                message += Environment.NewLine + exception.InnerException.Message
                    + Environment.NewLine + exception.InnerException.StackTrace;

                exception = exception.InnerException;
            }

            WriteLog(message, eventLogEntryType);
        }
    }
}
