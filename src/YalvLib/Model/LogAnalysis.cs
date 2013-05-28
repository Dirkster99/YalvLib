using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace YalvLib.Model
{
    
    /// <summary>
    /// This class is some acting as a layer where markers, custom filters and
    /// others features will be add to analyse the datas
    /// </summary>
    public class LogAnalysis
    {

        /// <summary>
        /// LogAnalysis Constructor
        /// </summary>
        public LogAnalysis()
        {
            Markers = new List<AbstractMarker>
                               {
                                   new ColorMarker(Color.BlueViolet),
                                   new ColorMarker(Color.Chocolate),
                                   new ColorMarker(Color.CadetBlue)
                               };
        }

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
        /// Add a textmarker linked to the given logentry list
        /// </summary>
        /// <param name="list">List of logEntry</param>
        /// <param name="marker">TextMarker</param>
        /// <returns>Markers</returns>
        public TextMarker AddTextMarker(List<LogEntry> list, TextMarker marker)
        {
            marker.LogEntries = list;
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
                if(marker.LogEntries.Remove(entry) && !IsMultiMarker(marker))
                    DeleteTextMarker(marker);
            }
        }

        /// <summary>
        /// Remove the given entry from the marker binding
        /// If the marker doesnt contains any entries after the removal
        /// the marker will be deleted
        /// </summary>
        /// <param name="entries">Entries to remove</param>
        public void RemoveTextMarker(List<LogEntry> entries)
        {
            foreach (TextMarker marker in TextMarkers)
            {
                foreach (LogEntry logEntry in entries)
                {
                    marker.LogEntries.Remove(logEntry);
                }
                if(!IsMultiMarker(marker))
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

        /// <summary>
        /// Return the list of TextMarkers linked to the given LogEntry
        /// </summary>
        /// <param name="entry">Log Entry</param>
        /// <returns>List of linked TextMarkers</returns>
        public List<TextMarker> GetTextMarkersForEntry(LogEntry entry)
        {
            return TextMarkers.Where(textMarker => textMarker.LogEntries.Contains(entry)).ToList();
        }

        /// <summary>
        /// Return the list of TextMarkers linked to the given list of logEntry
        /// </summary>
        /// <param name="logEntries">List of log entry</param>
        /// <returns>List of linked TextMarkers</returns>
        public List<TextMarker> GetTextMarkersForEntries(List<LogEntry> logEntries)
        {
            List<TextMarker> markers = new List<TextMarker>();
            foreach(LogEntry entry in logEntries)
            {
                markers.AddRange(GetTextMarkersForEntry(entry).Where(x=> !markers.Contains(x)));
            }
            return markers;
        }


        /// <summary>
        /// Determine if a textmarker is linked to the given log entry
        /// </summary>
        /// <param name="entry">LogEntry to check</param>
        /// <returns>true or false</returns>
        public bool ExistTextMarkerForLogEntry(LogEntry entry)
        {
            return GetTextMarkersForEntry(entry).Any();
        }

        /// <summary>
        /// Determine if a textmarker is linked to the given list of log entry
        /// </summary>
        /// <param name="logEntries">List of logEntry to check</param>
        /// <returns>true or false</returns>
        public bool ExistTextMarkerForLogEntries(List<LogEntry> logEntries)
        {
            return GetTextMarkersForEntries(logEntries).Any();
        }

        public void SetColorMarker(List<LogEntry> list, Color color)
        {
            GetColorMarker(color).LogEntries.AddRange(list);
        }

        public ColorMarker GetColorMarker(Color color)
        {
            return ColorMarkers.First(colorMarker => colorMarker.HighlightColor.Equals(color));
        }
    }
}
