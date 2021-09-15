using System;
using System.Collections.Generic;
using System.Text;

namespace NarfoxSparrow.Logging
{
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }

    public interface ILogger
    {
        LogLevel Level { get; set; }
        void Debug(string msg);
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg);
    }

    public class ConsoleLogger : ILogger
    {
        public LogLevel Level { get; set; } = LogLevel.Debug;

        public void Debug(string msg)
        {
            // swallow
        }

        public void Error(string msg)
        {
            // swallow
        }

        public void Info(string msg)
        {
            // swallow
        }

        public void Warn(string msg)
        {
            // swallow
        }
    }

    public class Log
    {
        private static ILogger log;

        public static ILogger Instance
        {
            get
            {
                if (log == null)
                {
                    log = new ConsoleLogger();
                }
                return log;
            }
            set
            {
                log = value;
            }
        }
    }
}
