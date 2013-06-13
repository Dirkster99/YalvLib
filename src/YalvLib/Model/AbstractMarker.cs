using System;
using System.Collections.Generic;
using System.Linq;

namespace YalvLib.Model
{
    /// <summary>
    /// A Marker is made to allow user pointing out some log entries in order to add comments, highlighting and so on...
    /// </summary>
    public abstract class AbstractMarker
    {
        #region fields

        private List<LogEntry> _linkedEntries;

        #endregion fields

        protected AbstractMarker(List<LogEntry> entries)
        {
            _linkedEntries = entries;
            DateCreation = DateTime.Now;
            DateLastModification = DateTime.Now;
        }

        protected AbstractMarker()
        {
        }

        /// <summary>
        /// return the timestamp of the marker yhen it has been created
        /// </summary>
        public DateTime DateCreation { get; private set; }

        /// <summary>
        /// Get/Set date of the last modification
        /// </summary>
        public DateTime DateLastModification { get; set; }


        /// <summary>
        /// Return logEntries linked with this marker
        /// </summary>
        public IList<LogEntry> LogEntries
        {
            get { return _linkedEntries; }
            set { _linkedEntries = value.ToList(); }
        }


        /// <summary>
        /// Return the number of logEntries binded with the current marker
        /// </summary>
        /// <returns></returns>
        public int LogEntryCount()
        {
            return _linkedEntries.Count;
        }

        /// <summary>
        /// Get/Set the Uid
        /// </summary>
        public Guid Uid { get; protected set; }
    }
}
