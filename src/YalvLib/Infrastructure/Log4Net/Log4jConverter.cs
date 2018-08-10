using System;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Log4Net
{
    /// <summary>
    /// Implements a converter to convert object values between
    ///   LogEntry (class) -> log4jEvent (event) and
    /// log4jEvent (event) -> LogEntry   (class)
    /// </summary>
    public class Log4jConverter
    {
        /// <summary>
        /// Shortcut string definition to determine the name of the application
        /// that was used to create the Log4Net entry.
        /// </summary>
        public const String AppKey = "log4japp";

        /// <summary>
        /// Shortcut string definition to determine the host name
        /// that was used to create the Log4Net entry.
        /// </summary>
        public const String HostKey = "log4net:HostName";

        /// <summary>
        /// Shortcut string definition to determine the name of the computer
        /// that was used to create the Log4Net entry.
        /// </summary>
        public const String MachineKey = "log4jmachinename";

        /// <summary>
        /// Shortcut string definition to determine the name of the user context
        /// that was used to create the Log4Net entry.
        /// </summary>
        public const String UserKey = "log4net:UserName";

        /// <summary>
        /// Method to convert a
        /// log4jEvent (event) into LogEntry (object).
        /// </summary>
        /// <param name="log4jEvent"></param>
        /// <returns></returns>
        public static LogEntry Convert(Event log4jEvent)
        {
            Event2LogEntry converter = new Event2LogEntry(log4jEvent);
            return converter.GetLogEntry();
        }

        /// <summary>
        /// Method to convert a
        /// LogEntry (object) -> log4jEvent (event).
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static Event Convert(LogEntry entry)
        {
            LogEntry2Event converter = new LogEntry2Event(entry);
            return converter.GetEvent();            
        }
    }
}
