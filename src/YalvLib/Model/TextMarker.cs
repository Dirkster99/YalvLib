namespace YalvLib.Model
{
    using log4netLib.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A TextMarker is used to add a comment on a Log entry
    /// </summary>
    public class TextMarker : AbstractMarker
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
        public TextMarker(List<ILogEntry> entries,
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
        #endregion properties
    }
}