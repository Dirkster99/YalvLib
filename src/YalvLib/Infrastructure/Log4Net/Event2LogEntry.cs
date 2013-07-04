using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Log4Net
{

    internal class Event2LogEntry
    {
        public static LogEntry Convert(Event log4jEvent)
        {
            Event2LogEntry converter = new Event2LogEntry(log4jEvent);
            return converter.GetLogEntry();
        }

        private Event _log4jEvent;
        private LogEntry _logEntry;


        internal Event2LogEntry(Event log4jEvent)
        {
            _log4jEvent = log4jEvent;
        }

        internal LogEntry GetLogEntry()
        {
            _logEntry = new LogEntry();

            _logEntry.LevelIndex = LevelConverter.From(_log4jEvent.Level);
            _logEntry.Message = _log4jEvent.Message;
            _logEntry.Logger = _log4jEvent.Logger;
            _logEntry.Thread = _log4jEvent.Thread;
            _logEntry.Throwable = _log4jEvent.Throwable;
            try
            {
                _logEntry.TimeStamp = DateTime.MinValue.AddMilliseconds(System.Convert.ToDouble(_log4jEvent.Timestamp)).ToLocalTime();
            }
            catch (Exception ex)
            {

                throw new Exception("Error converting timestamp field from log4j file", ex);
            }
            
            
            _logEntry.Class = _log4jEvent.LocationInfo.Class;
            _logEntry.File = _log4jEvent.LocationInfo.File;
            try
            {
                _logEntry.Line = System.Convert.ToUInt32(_log4jEvent.LocationInfo.Line);
            }catch(Exception ex)
            {
                if (_log4jEvent.LocationInfo.Line.Equals("?")){
                    _logEntry.Line = 0;
                }else{
                    throw new Exception("Error converting line number field from log4j file", ex);
                }
            }
            _logEntry.Method = _log4jEvent.LocationInfo.Method;

            _logEntry.App = _log4jEvent.Properties.First(x => x.Name.Equals(Log4jConverter.AppKey)).Value;
            _logEntry.HostName = _log4jEvent.Properties.First(x => x.Name.Equals(Log4jConverter.HostKey)).Value;
            _logEntry.MachineName = _log4jEvent.Properties.First(x => x.Name.Equals(Log4jConverter.MachineKey)).Value;
            _logEntry.UserName = _log4jEvent.Properties.First(x => x.Name.Equals(Log4jConverter.UserKey)).Value;

            return _logEntry;
        }

    }

}
