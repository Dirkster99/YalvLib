namespace log4netLib.Interfaces
{
    /// <summary>
    /// Defines properties and methods of a class that represents
    /// a row in the data grid.
    /// </summary>
    public interface ILogEntryRowViewModel
    {
        /// <summary>
        /// Get/Set the number of textmarkers linked to the entry
        /// </summary>
        int TextMarkerQuantity { get; set; }

        /// <summary>
        /// Get/Set the number of ColorMarkers linked to the entry
        /// </summary>
        int ColorMarkerQuantity { get; set; }

        /// <summary>
        /// Return the id of the linked logentry
        /// </summary>
        uint LogEntryId { get; }

        /// <summary>
        /// Get / set the linked logEntry
        /// </summary>
        ILogEntry Entry { get; }
    }
}