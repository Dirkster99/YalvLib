using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using YalvLib.Common;
using YalvLib.Common.Interfaces;
using YalvLib.Model;

namespace YalvLib.ViewModel
{
    /// <summary>
    /// This class contains every TextMarkerViewModels linked to the selected entries
    /// </summary>
    public class DisplayTextMarkersViewModel : BindableObject
    {

        private readonly ObservableCollection<TextMarkerViewModel> _textMarkerVmList;
        private TextMarkerViewModel _textMarkerAdd;
        private List<LogEntryRowViewModel> _selectedEntries;
        private bool _displayOnlyCommonMarkers;

        /// <summary>
        /// Constructor
        /// </summary>
        public DisplayTextMarkersViewModel()
        {
            _textMarkerAdd = new TextMarkerViewModel(new TextMarker(new List<LogEntry>(), string.Empty, string.Empty));
            _textMarkerVmList = new ObservableCollection<TextMarkerViewModel>();
            _textMarkerAdd.CommandChangeTextMarker.Executed += ExecuteChange;
            CommandUpdateTextMarkers = new CommandRelay(CommandUpdateTextMarkersExecute, CommandUpdateTextMarkersCanExecute);
        }

        /// <summary>
        /// Generate a list of TextMarkerViewModel 
        /// from the TextMarker list given
        /// </summary>
        /// <param name="tm">TextMarker list</param>
        public void GenerateViewModels(List<TextMarker> tm)
        {
            _textMarkerVmList.Clear();
            GetNewTextMarkerToAdd();
            foreach (TextMarker textMarker in tm)
            {
                _textMarkerVmList.Add(new TextMarkerViewModel(textMarker));
            }
            foreach (var textMarkerViewModel in _textMarkerVmList)
            {
                textMarkerViewModel.CommandCancelTextMarker.Executed += ExecuteCancel;
            }
        }

        /// <summary>
        /// Get/Set for the TextmarkerViewModel row used
        /// to add a new TextMarker
        /// </summary>
        public TextMarkerViewModel TextMarkerToAdd
        {
            get { return _textMarkerAdd; }
            private set { _textMarkerAdd = value; NotifyPropertyChanged(() => TextMarkerToAdd);}
        }


        /// <summary>
        /// Getter for the TextMarkerViewModel list
        /// </summary>
        public ObservableCollection<TextMarkerViewModel> TextMarkerViewModels
        {
            get
            {
                return _textMarkerVmList;
            }
        }

        /// <summary>
        /// Getter / Setter for the list of entries
        /// selected by the user
        /// </summary>
        public List<LogEntryRowViewModel> SelectedEntries
        {
            get { return _selectedEntries; }
            private set { _selectedEntries = value; }
        }

        /// <summary>
        /// Getter / Setter for the Checkbox used
        /// to know if we have to display every markers for the selected
        /// entries or only the common ones
        /// </summary>
        public bool DisplayOnlyCommonMarkers
        {
            get { return _displayOnlyCommonMarkers; }
            private set { _displayOnlyCommonMarkers = value;
                if(_selectedEntries != null)
                    CommandUpdateTextMarkersExecute(_selectedEntries);
            }
        }

        public ICommandAncestor CommandUpdateTextMarkers { get; protected set; }

        /// <summary>
        /// Update the list of TextMarkerViewModel
        /// from the selected entries and the displayOnlyCommonMarker value
        /// </summary>
        /// <param name="arg">List of selected log entries</param>
        /// <returns>null</returns>
        internal object CommandUpdateTextMarkersExecute(object arg)
        {
            _selectedEntries = new List<LogEntryRowViewModel>((IEnumerable<LogEntryRowViewModel>)arg);

            IEnumerable<TextMarker> markers =
                YalvRegistry.Instance.ActualWorkspace.Analysis.GetTextMarkersForEntries(_selectedEntries.Select(x => x.Entry));

            List<TextMarker> markersCommon = markers.Where(
                x =>
                _selectedEntries.All(
                    e => YalvRegistry.Instance.ActualWorkspace.Analysis.GetTextMarkersForEntry(e.Entry).Contains(x))).ToList();

            GenerateViewModels(DisplayOnlyCommonMarkers ? markersCommon : markers.ToList());
            return null;
        }

        /// <summary>
        /// Generate a new TextMarkerViewModel used to
        /// add TextMarker
        /// </summary>
        private void GetNewTextMarkerToAdd()
        {
            _textMarkerAdd.CommandChangeTextMarker.Executed -= ExecuteChange;
            TextMarkerToAdd = new TextMarkerViewModel(new TextMarker(new List<LogEntry>(), string.Empty, string.Empty));
            _textMarkerAdd.CommandChangeTextMarker.Executed += ExecuteChange;
        }


        internal bool CommandUpdateTextMarkersCanExecute(object obj)
        {
            return ((IEnumerable<LogEntryRowViewModel>)obj).Any();
        }

        private void ExecuteChange(object sender, EventArgs e)
        {
            _textMarkerVmList.Add(TextMarkerToAdd);
            YalvRegistry.Instance.ActualWorkspace.Analysis.AddTextMarker(_selectedEntries.Select(x => x.Entry), TextMarkerToAdd.Marker);
            TextMarkerToAdd.CommandCancelTextMarker.Executed += ExecuteCancel;
            GetNewTextMarkerToAdd();
        }

        private void ExecuteCancel(object sender, EventArgs e)
        {
            CommandUpdateTextMarkersExecute(_selectedEntries);
        }

    }
}
