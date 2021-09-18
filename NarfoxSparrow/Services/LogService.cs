using System;
using System.Collections.Generic;
using System.Text;

namespace NarfoxSparrow.Services
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
            Write(LogLevel.Debug, msg);
        }

        public void Info(string msg)
        {
            Write(LogLevel.Info, msg);
        }

        public void Warn(string msg)
        {
            Write(LogLevel.Warn, msg);
        }

        public void Error(string msg)
        {
            Write(LogLevel.Error, msg);
        }

        void Write(LogLevel level, string msg)
        {
            if (Level <= level)
            {
                var timestamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                Console.WriteLine($"{level} ({timestamp}): {msg}");
            }
        }
    }

    public class LogService
    {
        private static ILogger log;

        public static ILogger Instance
        {
            get
            {
                if (log == null)
                {
                    log = new ConsoleLogger();
                    log.Level = LogLevel.Debug;
                }
                return log;
            }
            set
            {
                log = value;
            }
        }

        private LogService() { }
    }
}
