using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YalvLib.Common;
using YalvLib.Providers;

namespace YalvLib.Model
{

    public class LogEntryRepository : object
    {
        public string Path;
        public bool Active;
        private List<LogEntry> _logEntries = new List<LogEntry>();

        public IList<LogEntry> LogEntries
        {
            get { return _logEntries; }
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
            {
                entry.Id = _lastLogEntry.Id + 1;
               
            }
            else{
                entry.Id = 1;
            }
            entry.GuId = YalvRegistry.Instance.generateGuid();
        }

        private void AssignDelta(LogEntry entry)
        {
            DateTime comparisonDateTime = entry.TimeStamp;            
            if (_lastLogEntry != null)
                comparisonDateTime = _lastLogEntry.TimeStamp;
            entry.Delta = GlobalHelper.GetTimeDelta(comparisonDateTime, entry.TimeStamp);
        }

        public override bool Equals(object obj)
        {
            var item = obj as LogEntryRepository;
            if(item == null)
            {
                return false;
            }
            if (this.LogEntries.Count != item.LogEntries.Count)
                return false;
            return !this.LogEntries.Where((t, i) => !t.Equals(item.LogEntries[i])).Any();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
