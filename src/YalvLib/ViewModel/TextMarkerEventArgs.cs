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
        private TextMarker _tm;

        public TextMarkerEventArgs(TextMarker tm)
        {
            _tm = tm;
        }

        public TextMarker TextMarker { get { return _tm; } }

    }
}
