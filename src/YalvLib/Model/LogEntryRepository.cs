using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace YalvLib.Model
{

    public class LogEntryRepository
    {
        protected List<LogEntry> _logEntries = new List<LogEntry>();

        public ReadOnlyCollection<LogEntry> LogEntries
        {
            get { return new ReadOnlyCollection<LogEntry>(_logEntries); }
        }

        public void AddLogEntry(LogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            _logEntries.Add(entry);
        }
    }

}
