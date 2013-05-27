using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YalvLib.Common;
using YalvLib.Common.Interfaces;
using YalvLib.Model;

namespace YalvLib.ViewModel
{
    public class DisplayTextMarkersViewModel : BindableObject
    {

        private readonly ObservableCollection<TextMarkerViewModel> _textMarkerVmList;
        private List<LogEntry> _selectedEntries;

        public DisplayTextMarkersViewModel()
        {
            _textMarkerVmList = new ObservableCollection<TextMarkerViewModel>();
            CommandUpdateTextMarkers = new CommandRelay(CommandUpdateTextMarkersExecute, CommandUpdateTextMarkersCanExecute);
        }

        public void GenerateViewModels(List<TextMarker> tm)
        {
            foreach (TextMarker textMarker in tm)
            {
                _textMarkerVmList.Add(new TextMarkerViewModel(textMarker));
            }
            //RaisePropertyChanged("TextMarkerViewModels");
        }

        public ObservableCollection<TextMarkerViewModel> TextMarkerViewModels
        {
            get
            {
                return _textMarkerVmList;
            }
        }

        public bool HasData
        {
            get { return _textMarkerVmList.Count >= 1; }
        }

        public List<LogEntry> SelectedEntries
        {
            get { return _selectedEntries; }
            private set { _selectedEntries = value; }
        }

        public ICommandAncestor CommandUpdateTextMarkers { get; protected set; }


        internal object CommandUpdateTextMarkersExecute(object arg)
        {
            List<TextMarker> tm = new List<TextMarker>();
            _selectedEntries = new List<LogEntry>((IEnumerable<LogEntry>)arg);
            foreach (LogEntry e in ((IEnumerable<LogEntry>)arg))
            {
                tm.AddRange(YalvRegistry.Instance.ActualWorkspace.Analysis.GetTextMarkersForEntry(e));
            }
            GenerateViewModels(tm);
            return null;
        }


        internal bool CommandUpdateTextMarkersCanExecute(object obj)
        {
            return ((IEnumerable<LogEntry>)obj).Any();
        }
    }
}
