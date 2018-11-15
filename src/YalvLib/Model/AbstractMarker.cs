namespace YalvLib.Model
{
    using log4netLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A Marker is made to allow user pointing out some log entries in order to add comments,
    /// highlighting and so on...
    /// </summary>
    public abstract class AbstractMarker
    {
        #region fields
        private List<ILogEntry> _linkedEntries;
        #endregion fields

        #region Constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="entries"></param>
        protected AbstractMarker(List<ILogEntry> entries)
        {
            _linkedEntries = entries;
            DateCreation = DateTime.Now;
            DateLastModification = DateTime.Now;
        }

        /// <summary>
        /// Hidden default class constructor
        /// </summary>
        protected AbstractMarker()
        {
        }
        #endregion  Constructors

        #region Properties
        /// <summary>
        /// return the timestamp of the marker yhen it has been created
        /// </summary>
        public DateTime DateCreation { get; set; }

        /// <summary>
        /// Get/Set date of the last modification
        /// </summary>
        public DateTime DateLastModification { get; set; }

        /// <summary>
        /// Return logEntries linked with this marker
        /// </summary>
        public IList<ILogEntry> LogEntries
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
        public Guid Uid { get; set; }
        #endregion Properties
    }
}
