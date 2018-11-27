namespace YalvLib.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A TextMarker is used to add a comment on a Log entry
    /// </summary>
    public class TextMarker : AbstractMarker, ICloneable
    {
        #region fields
        private string _author;
        private string _message;
        #endregion fields

        #region constructors
        /// <summary>
        /// TextMarker constructor
        /// </summary>
        /// <param name="entries">Linked LogEntries</param>
        /// <param name="author">Author of the Marker</param>
        /// <param name="message">Given message</param>
        public TextMarker(List<LogEntry> entries,
                          string author, string message)
            : base(entries)
        {
            _author = author;
            _message = message;
        }

        /// <summary>
        /// Empty constructor for nhibernate
        /// </summary>
        public TextMarker()
        {
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Get or Set the author. Update Date of last modification on get
        /// </summary>
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                DateLastModification = DateTime.Now;
            }
        }

        /// <summary>
        /// Get or Set the message. Update Date of last modification on get
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                DateLastModification = DateTime.Now;
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var ret = new TextMarker(LogEntries.ToList(), Author, Message);

            ret.Uid = Uid;                        // Implement exact clone
            ret.DateCreation = DateCreation;
            ret.DateLastModification = DateLastModification;

            return ret;
        }
        #endregion properties
    }
}