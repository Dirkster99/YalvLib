using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Model;

namespace YalvLib.ViewModel
{
    /// <summary>
    /// This class represent a special type of event. It allow the YalvViewModel to know 
    /// wich textmarker is concerned with the changed applied on it
    /// </summary>
    public class TextMarkerEventArgs : EventArgs
    {
        private readonly TextMarker _tm;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tm">Textmarker associated with the event</param>
        public TextMarkerEventArgs(TextMarker tm)
        {
            _tm = tm;
        }

        /// <summary>
        /// Getter textmarker associated with the event
        /// </summary>
        public TextMarker TextMarker { get { return _tm; } }

    }
}
