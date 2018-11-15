namespace YalvLib.Model
{
    using log4netLib.Interfaces;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Color Marker is used to highlighted specified log entries
    /// </summary>
    public class ColorMarker : AbstractMarker
    {
        /// <summary>
        /// Constructor based on AbstractMarker constructor
        /// </summary>
        /// <param name="entries">Entries linked to the marker</param>
        /// <param name="color">highlight color</param>
        public ColorMarker(List<ILogEntry> entries, Color color)
            : base(entries)
        {
            HighlightColor = color;
        }

        /// <summary>
        /// Constructor with empty logentries.
        /// ColorMarkers will be created as the app starts
        /// </summary>
        /// <param name="color">Color of the ColorMarker</param>
        public ColorMarker(Color color)
            : this(new List<ILogEntry>(), color)
        {
        }

        /// <summary>
        /// Empty constructor for the fluentnhibernate mapping
        /// </summary>
        public ColorMarker()
        {
        }

        /// <summary>
        /// Get color
        /// </summary>
        public Color HighlightColor { get; private set; }
    }
}
