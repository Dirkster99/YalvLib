using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YalvLib.Common;
using YalvLib.Providers;

namespace YalvLib.Model
{

    public class LogEntryRepository
    {

        private List<LogEntry> _logEntries = new List<LogEntry>();

        public IList<LogEntry> LogEntries
        {
            //get { return new ReadOnlyCollection<LogEntry>(_logEntries); }
            get { return _logEntries; }
            protected set {}
        }

        public Guid Uid { get; set; }

        public void AddLogEntry(LogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            AssignId(entry);
            AssignDelta(entry);
            _logEntries.Add(entry);
            _lastLogEntry = entry;
        }

        public void AddLogEntries(IEnumerable<LogEntry> logEntries)
        {
            foreach (var entry in logEntries)
            {
                AddLogEntry(entry);
            }            
        }

        private LogEntry _lastLogEntry;

        private void AssignId(LogEntry entry)
        {
            if (_lastLogEntry != null)
                entry.Id = _lastLogEntry.Id + 1;
            else
                entry.Id = 1;
        }

        private void AssignDelta(LogEntry entry)
        {
            DateTime comparisonDateTime = entry.TimeStamp;            
            if (_lastLogEntry != null)
                comparisonDateTime = _lastLogEntry.TimeStamp;
            entry.Delta = GlobalHelper.GetTimeDelta(comparisonDateTime, entry.TimeStamp);
        }
    }

}
