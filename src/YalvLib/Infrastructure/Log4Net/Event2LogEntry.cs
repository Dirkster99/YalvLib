namespace YalvLib.Infrastructure.Log4Net
{
    using YalvLib.Model;
    using System;
    using System.Linq;

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
                var dTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                var doubleMilliSecs = Double.Parse(_log4jEvent.Timestamp);
                _logEntry.TimeStamp = dTime.AddMilliseconds(doubleMilliSecs).ToLocalTime();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error converting timestamp field '{0}' from log4j file",
                                        (_log4jEvent.Timestamp != null ? _log4jEvent.Timestamp : "(null)"))
                                        , ex);
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
