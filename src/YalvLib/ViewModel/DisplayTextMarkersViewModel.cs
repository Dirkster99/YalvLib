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
        private TextMarkerViewModel _textMarkerAdd;
        private List<LogEntry> _selectedEntries;

        public DisplayTextMarkersViewModel()
        {
            _textMarkerAdd = new TextMarkerViewModel(new TextMarker(new List<LogEntry>(), string.Empty, string.Empty), this);
            _textMarkerVmList = new ObservableCollection<TextMarkerViewModel>();
            _textMarkerAdd.CommandChangeTextMarker.Executed += ExecuteChange;
            CommandUpdateTextMarkers = new CommandRelay(CommandUpdateTextMarkersExecute, CommandUpdateTextMarkersCanExecute);
        }


        public void GenerateViewModels(List<TextMarker> tm)
        {
            _textMarkerVmList.Clear();
            GetNewTextMarkerToAdd();
            foreach (TextMarker textMarker in tm)
            {
                _textMarkerVmList.Add(new TextMarkerViewModel(textMarker, this));
            }
            foreach (var textMarkerViewModel in _textMarkerVmList)
            {
                textMarkerViewModel.CommandCancelTextMarker.Executed += ExecuteCancel;
            }
        }


        public TextMarkerViewModel TextMarkerToAdd
        {
            get { return _textMarkerAdd; }
            private set { _textMarkerAdd = value; RaisePropertyChanged("TextMarkerToAdd"); }
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
               foreach (TextMarker tmE in YalvRegistry.Instance.ActualWorkspace.Analysis.GetTextMarkersForEntry(e).Where(tmE => !tm.Contains(tmE)))
               {
                   tm.Add(tmE);
               }
            }
            GenerateViewModels(tm);
            return null;
        }

        private void GetNewTextMarkerToAdd()
        {
            _textMarkerAdd.CommandChangeTextMarker.Executed -= ExecuteChange;
            TextMarkerToAdd = new TextMarkerViewModel(new TextMarker(new List<LogEntry>(), string.Empty, string.Empty), this);
            _textMarkerAdd.CommandChangeTextMarker.Executed += ExecuteChange;
        }


        internal bool CommandUpdateTextMarkersCanExecute(object obj)
        {
            return ((IEnumerable<LogEntry>)obj).Any();
        }

        private void ExecuteChange(object sender, EventArgs e)
        {
            _textMarkerVmList.Add(TextMarkerToAdd);
            YalvRegistry.Instance.ActualWorkspace.Analysis.AddTextMarker(_selectedEntries, TextMarkerToAdd.Marker);
            TextMarkerToAdd.CommandCancelTextMarker.Executed += ExecuteCancel;
            GetNewTextMarkerToAdd();
        }


        private void ExecuteCancel(object sender, EventArgs e)
        {
            CommandUpdateTextMarkersExecute(_selectedEntries);
        }

    }
}
