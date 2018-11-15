namespace YalvLib.ViewModels
{
    using log4netLib.Interfaces;
    using YalvLib.Common;
    using YalvLib.Model;

    /// <summary>
    /// This class represent a row in the data grid
    /// </summary>
    public class LogEntryRowViewModel : BindableObject, ILogEntryRowViewModel
    {
        private int _colorMarkerQuantity;
        private ILogEntry _entry;
        private int _textMarkerquantity;

        /// <summary>
        /// Constructor based on a logentry
        /// </summary>
        /// <param name="item"></param>
        public LogEntryRowViewModel(LogEntry item)
        {
            Entry = item;
        }

        /// <summary>
        /// Get/Set the number of textmarkers linked to the entry
        /// </summary>
        public int TextMarkerQuantity
        {
            get { return _textMarkerquantity; }
            set
            {
                _textMarkerquantity = value;
                NotifyPropertyChanged(() => TextMarkerQuantity);
            }
        }

        /// <summary>
        /// Get/Set the number of ColorMarkers linked to the entry
        /// </summary>
        public int ColorMarkerQuantity
        {
            get { return _colorMarkerQuantity; }
            set
            {
                _colorMarkerQuantity = value;
                NotifyPropertyChanged(() => ColorMarkerQuantity);
            }
        }


        /// <summary>
        /// Return the id of the linked logentry
        /// </summary>
        public uint LogEntryId
        {
            get { return _entry.Id; }
        }


        /// <summary>
        /// Get / set the linked logEntry
        /// </summary>
        public ILogEntry Entry
        {
            get { return _entry; }
            private set
            {
                _entry = value;
            }
        }

        internal void UpdateTextMarkerQuantity()
        {
            TextMarkerQuantity =
                YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.GetTextMarkersForEntry(Entry).Count;
        }
    }
}