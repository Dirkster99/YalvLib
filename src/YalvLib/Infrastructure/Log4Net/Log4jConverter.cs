using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Log4Net
{
    public class Log4jConverter
    {

        public const String AppKey = "log4japp";
        public const String HostKey = "log4net:HostName";
        public const String MachineKey = "log4jmachinename";
        public const String UserKey = "log4net:UserName";

        public static LogEntry Convert(Event log4jEvent)
        {
            Event2LogEntry converter = new Event2LogEntry(log4jEvent);
            return converter.GetLogEntry();
        }
        public static Event Convert(LogEntry entry)
        {
            LogEntry2Event converter = new LogEntry2Event(entry);
            return converter.GetEvent();            
        }
    }
}
