using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Model;

namespace YalvLib.ViewModel
{
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
