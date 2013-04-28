using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Log4Net
{

    internal class LogEntry2Event
    {

        public static Event Convert(LogEntry entry)
        {
            LogEntry2Event converter = new LogEntry2Event(entry);
            return converter.GetEvent();
        }

        private Event _log4jEvent;
        private LogEntry _logEntry;


        internal LogEntry2Event(LogEntry entry)
        {
            _logEntry = entry;
        }

        internal Event GetEvent()
        {
            _log4jEvent = new Event();

            _log4jEvent.Level = _logEntry.LevelIndex.ToString().ToUpper();
            _log4jEvent.Message = _logEntry.Message;
            _log4jEvent.Logger = _logEntry.Logger;
            _log4jEvent.Thread = _logEntry.Thread;
            _log4jEvent.Throwable = _logEntry.Throwable;
            _log4jEvent.Timestamp = (_logEntry.TimeStamp - DateTime.MinValue).TotalMilliseconds.ToString();

            _log4jEvent.LocationInfo = new LocationInfo();
            _log4jEvent.LocationInfo.Class = _logEntry.Class;
            _log4jEvent.LocationInfo.File = _logEntry.File;
            _log4jEvent.LocationInfo.Line = _logEntry.Line.ToString();
            _log4jEvent.LocationInfo.Method = _logEntry.Method;

            _log4jEvent.Properties.Add(new Data()
            {
                Name = Log4jConverter.AppKey,
                Value = _logEntry.App
            });
            _log4jEvent.Properties.Add(new Data()
            {
                Name = Log4jConverter.HostKey,
                Value = _logEntry.HostName
            });
            _log4jEvent.Properties.Add(new Data()
            {
                Name = Log4jConverter.MachineKey,
                Value = _logEntry.MachineName
            });
            _log4jEvent.Properties.Add(new Data()
            {
                Name = Log4jConverter.UserKey,
                Value = _logEntry.UserName
            });
            return _log4jEvent;
        }

    }

}
