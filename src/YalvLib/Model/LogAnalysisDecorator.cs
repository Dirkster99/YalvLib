using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace YalvLib.Model
{
    
    /// <summary>
    /// Class name is not definitive, I don't think it's a clear one.
    /// This class is some kind of layer where markers, custom filters and
    /// others features will be add
    /// </summary>
    public class LogAnalysisDecorator
    {

        //private static LogAnalysisDecorator _singleton = null;
        //static readonly object padlock = new object();

        /// <summary>
        /// LogAnalysisDecorator Constructor
        /// </summary>
        public LogAnalysisDecorator()
        {
            Markers = new List<AbstractMarker>
                               {
                                   new ColorMarker(Color.BlueViolet),
                                   new ColorMarker(Color.Chocolate),
                                   new ColorMarker(Color.CadetBlue)
                               };
        }

        /*public static LogAnalysisDecorator Instance
        {
            get
            {
                lock(padlock)
                {
                    return _singleton ?? (_singleton = new LogAnalysisDecorator());
                }
            }
        }*/
       

        /// <summary>-
        /// Get / Set Markers list
        /// </summary>
        public List<AbstractMarker> Markers
        {
            get; private set;
        }

        /// <summary>
        /// Return the list of TextMarkers
        /// </summary>
        public List<TextMarker> TextMarkers
        {
            get { return Markers.Where(x => x is TextMarker).Cast<TextMarker>().ToList(); }
        }

        /// <summary>
        /// Return the list of ColorMarkers
        /// </summary>
        public List<ColorMarker> ColorMarkers
        {
            get { return Markers.Where(x => x is ColorMarker).Cast<ColorMarker>().ToList(); }
        }

        /// <summary>
        /// Adds a textMarker to the Markers list
        /// </summary>
        /// <param name="list">Entries that will be binded to the TextMarker</param>
        /// <param name="author">Author of the TextMarker</param>
        /// <param name="message">Message of the Textmarker</param>
        /// <returns></returns>
        public TextMarker AddTextMarker(List<LogEntry> list, string author, string message)
        {
            TextMarker marker = new TextMarker(list, author, message);
            Markers.Add(marker);
            return marker;
        }

        /// <summary>
        /// Check is the numbers of entries binded to the given Marker
        /// </summary>
        /// <param name="marker">Marker to check</param>
        /// <returns>True if some entries are binded to the marker, false otherwise</returns>
        public bool IsMultiMarker(TextMarker marker)
        {
            return marker.LogEntries.Count > 0;
        }

        /// <summary>
        /// Remove the given entry from the marker binding
        /// If the marker doesnt contains any entries after the removal
        /// the marker will be deleted
        /// </summary>
        /// <param name="entry">Entry to remove</param>
        public void RemoveTextMarker(LogEntry entry)
        {
            foreach (TextMarker marker in TextMarkers)
            {   
                if(marker.RemoveEntry(entry) && !IsMultiMarker(marker))
                    DeleteTextMarker(marker);
            }
        }

        /// <summary>
        /// Delete the given TextMarker
        /// </summary>
        /// <param name="marker">TextMarker to be deleted</param>
        public void DeleteTextMarker(TextMarker marker)
        {
            Markers.Remove(marker);
        }
    }
}
