using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YalvLib.Common;
using YalvLib.Model;

namespace YalvLib.ViewModel
{
    public class LogEntryRowViewModel : BindableObject
    {    
        
        private int _colorMarkerQuantity;
        private int _textMarkerquantity;
        private LogEntry _entry;


        public LogEntryRowViewModel(LogEntry item)
        {
            Entry = item;
        }

        public int TextMarkerQuantity
        {
            get { return _textMarkerquantity; }
            set
            {
                _textMarkerquantity = value;
                NotifyPropertyChanged(() => TextMarkerQuantity);
            }
        }
        public int ColorMarkerQuantity
        {
            get { return _colorMarkerQuantity; }
            set { _colorMarkerQuantity = value; NotifyPropertyChanged(() => ColorMarkerQuantity); }
        }

        public uint LogEntryId
        {
            get { return _entry.Id; }
        }

        public LogEntry Entry
        {
            get { return _entry; }
            private set { _entry = value; }
        }

        internal void UpdateTextMarkerQuantity()
        {
            TextMarkerQuantity = YalvRegistry.Instance.ActualWorkspace.Analysis.GetTextMarkersForEntry(Entry).Count;
        }
    }
}
