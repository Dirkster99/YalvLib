using System;
using System.Collections.Generic;
using System.Linq;
using YalvLib.Common;

namespace YalvLib.Model
{
    /// <summary>
    /// This class represent a repository of log entries
    /// </summary>
    public class LogEntryRepository : object
    {
        private readonly List<LogEntry> _logEntries = new List<LogEntry>();

        /// <summary>
        /// Boolean telling if the log entries of this repo have to be displayed
        /// </summary>
        public bool Active;

        /// <summary>
        /// Path of the repository
        /// </summary>
        public string Path;

        private LogEntry _lastLogEntry;

        /// <summary>
        /// Getter of the list of log entries contained in the file
        /// </summary>
        public IList<LogEntry> LogEntries
        {
            get { return _logEntries; }
        }

        /// <summary>
        /// The uniaue identifier of this instance (used for sql mapping)
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Add a log entry to the repository
        /// </summary>
        /// <param name="entry"></param>
        public void AddLogEntry(LogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            AssignId(entry);
            AssignDelta(entry);
            _logEntries.Add(entry);
            _lastLogEntry = entry;
        }

        /// <summary>
        /// Add a list a log entries to the repository
        /// </summary>
        /// <param name="logEntries"></param>
        public void AddLogEntries(IEnumerable<LogEntry> logEntries)
        {
            foreach (LogEntry entry in logEntries)
            {
                AddLogEntry(entry);
            }
        }

        private void AssignId(LogEntry entry)
        {
            if (_lastLogEntry != null)
            {
                entry.Id = _lastLogEntry.Id + 1;
            }
            else
            {
                entry.Id = 1;
            }
            entry.GuId = YalvRegistry.Instance.GenerateGuid();
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
            if (item == null)
            {
                return false;
            }
            if (LogEntries.Count != item.LogEntries.Count)
                return false;
            return !LogEntries.Where((t, i) => !t.Equals(item.LogEntries[i])).Any();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}