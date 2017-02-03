using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GrowBoxServer
{
    delegate string LogFormatterCallback(LogMessage message);

    /// <summary>
    /// The verbosity of the logging
    /// </summary>
    enum LoggingLevel : int
    {
        NORMAL = 0,
        VERBOSE = 1,
        DEBUG = 2
    }

    /// <summary>
    /// The type of the logging
    /// </summary>
    enum LogginType : int
    {
        INFO = 0,
        WARNING = 1,
        ERROR = 2
    }

    /// <summary>
    /// Represents a log message
    /// </summary>
    class LogMessage
    {
        /// <summary>
        /// Message text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Logging level for this message
        /// </summary>
        public LoggingLevel Level { get; set; }

        /// <summary>
        /// Log type
        /// </summary>
        public LogginType Type { get; set; }

        public LogMessage(string message, LoggingLevel level, LogginType type)
        {
            Message = message;
            Level = level;
            Type = type;
        }
    }

    /// <summary>
    /// Base class for all loggers
    /// </summary>
    abstract class Logger
    {
        /// <summary>
        /// Enable or disable this logger
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Logging level (verbosity)
        /// </summary>
        public LoggingLevel Level { get; set; } = LoggingLevel.NORMAL;

        /// <summary>
        /// Method to call to get the formatted log message
        /// </summary>
        public LogFormatterCallback LogFormatter { get; set; } = StandardFormatter;

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">message to log</param>
        public abstract void Log(LogMessage message);

        protected bool Filter(LogMessage message)
        {
            return IsEnabled && message.Level <= Level;
        }

        /// <summary>
        /// A standard formatting for log messages
        /// </summary>
        /// <param name="message">Message to format</param>
        /// <returns>Formatted message</returns>
        public static string StandardFormatter(LogMessage message)
        {
            return string.Format("[{0}] {1}", message.Type, message.Message);
        }
    }

    /// <summary>
    /// Logs to the console
    /// </summary>
    class ConsoleLogger : Logger
    {
        public override void Log(LogMessage message)
        {
            if (Filter(message))
                Console.WriteLine(LogFormatter(message));
        }
    }

    /// <summary>
    /// Logs to the specified file
    /// </summary>
    class FileLogger : Logger
    {
        /// <summary>
        /// Path to the file to write log to
        /// </summary>
        public string FileName { get; private set; }

        public FileLogger(string filename, bool eraseFirst = false)
        {
            FileName = filename;
            if (eraseFirst && File.Exists(FileName))
                File.WriteAllText(FileName, string.Empty);
        }

        public override void Log(LogMessage message)
        {
            if (Filter(message))
                File.AppendAllLines(FileName, new string[] { LogFormatter(message) });
        }
    }

    /// <summary>
    /// Gives you the possibility to use more loggers at once
    /// </summary>
    static class LoggerContainer
    {
        public static bool IsEnabled { get; set; } = true;
        public static List<Logger> Loggers { get; private set; } = new List<Logger>();

        /// <summary>
        /// Logs the message to all loggers in Loggers property
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void Log(LogMessage message)
        {
            if (IsEnabled)
                foreach (Logger l in Loggers)
                    l.Log(message);
        }
    }
}
