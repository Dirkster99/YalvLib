using System;
using System.Collections.Generic;

namespace YalvLib.Model
{
    /// <summary>
    /// A TextMarker is used to add a comment on a Log entry
    /// </summary>
    public class TextMarker : AbstractMarker
    {
        #region fields

        private string _author;
        private string _message;

        #endregion fields

        /// <summary>
        /// TextMarker constructor
        /// </summary>
        /// <param name="entries">Linked LogEntries</param>
        /// <param name="author">Author of the Marker</param>
        /// <param name="message">Given message</param>
        public TextMarker(List<LogEntry> entries, string author, string message)
            : base(entries)
        {
            _author = author;
            _message = message;
        }

        public TextMarker()
        {
        }


        /// <summary>
        /// Get or Set the author. Update Date of last modification on get
        /// </summary>
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                DateLastModification = new DateTime();
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
                DateLastModification = new DateTime();
            }
        }

    }
}