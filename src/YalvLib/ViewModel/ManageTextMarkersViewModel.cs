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
    public class ManageTextMarkersViewModel : BindableObject, IManageTextMarkersViewModel
    {

        private readonly ObservableCollection<TextMarkerViewModel> _textMarkerVmList;
        private TextMarkerViewModel _textMarkerAdd;
        private List<LogEntryRowViewModel> _selectedEntries;
        private bool _displayOnlyCommonMarkers;

        /// <summary>
        /// Constructor
        /// </summary>
        public ManageTextMarkersViewModel()
        {
            _textMarkerAdd = new TextMarkerViewModel(new TextMarker(new List<LogEntry>(), string.Empty, string.Empty));
            _textMarkerVmList = new ObservableCollection<TextMarkerViewModel>();
            _textMarkerAdd.CommandChangeTextMarker.Executed += ExecuteChange;
            _textMarkerAdd.TextMarkerDeleted += ExecuteCancel;
            CommandUpdateTextMarkers = new CommandRelay(CommandUpdateTextMarkersExecute, CommandUpdateTextMarkersCanExecute);
        }

        /// <summary>
        /// Generate a list of TextMarkerViewModel 
        /// from the TextMarker list given
        /// </summary>
        /// <param name="textMarkerList">TextMarker list</param>
        public void GenerateViewModels(List<TextMarker> textMarkerList)
        {
            foreach (var textMarkerViewModel in TextMarkerViewModels)
            {
                textMarkerViewModel.TextMarkerDeleted -= ExecuteCancel;
            }

            TextMarkerViewModels.Clear();
            GetNewTextMarkerToAdd();

            foreach (TextMarker textMarker in textMarkerList)
            {
                TextMarkerViewModels.Add(new TextMarkerViewModel(textMarker));
            }
            foreach (var textMarkerViewModel in TextMarkerViewModels)
            {
                textMarkerViewModel.TextMarkerDeleted += ExecuteCancel;
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
            set { _selectedEntries = value; }
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
                YalvRegistry.Instance.ActualWorkspace.currentAnalysis.GetTextMarkersForEntries(_selectedEntries.Select(x => x.Entry));

            List<TextMarker> markersCommon = markers.Where(
                x =>
                _selectedEntries.All(
                    e => YalvRegistry.Instance.ActualWorkspace.currentAnalysis.GetTextMarkersForEntry(e.Entry).Contains(x))).ToList();

            GenerateViewModels(DisplayOnlyCommonMarkers ? markersCommon : markers.ToList());
            return null;
        }

        /// <summary>
        /// Generate a new TextMarkerViewModel used to
        /// add TextMarker
        /// </summary>
        private void GetNewTextMarkerToAdd()
        {
            TextMarkerToAdd.TextMarkerDeleted -= ExecuteCancel;
            TextMarkerToAdd.CommandChangeTextMarker.Executed -= ExecuteChange;
            TextMarkerToAdd = new TextMarkerViewModel(new TextMarker(new List<LogEntry>(), string.Empty, string.Empty));
            TextMarkerToAdd.CommandChangeTextMarker.Executed += ExecuteChange;
            TextMarkerToAdd.TextMarkerDeleted += ExecuteCancel;
        }


        internal bool CommandUpdateTextMarkersCanExecute(object obj)
        {
            return ((IEnumerable<LogEntryRowViewModel>)obj).Any();
        }

        public void ExecuteChange(object sender, EventArgs e)
        {
            TextMarkerViewModels.Add(TextMarkerToAdd);
            YalvRegistry.Instance.ActualWorkspace.currentAnalysis.AddTextMarker(_selectedEntries.Select(x => x.Entry), TextMarkerToAdd.Marker);
            foreach(LogEntryRowViewModel entry in _selectedEntries)
            {
                entry.UpdateTextMarkerQuantity();
            }
            MarkerAdded(this, null);
            GetNewTextMarkerToAdd();
        }

        public void ExecuteCancel(object obj, EventArgs eventArgs)
        {
            TextMarkerEventArgs args = eventArgs as TextMarkerEventArgs;
            YalvRegistry.Instance.ActualWorkspace.currentAnalysis.DeleteTextMarker(args.TextMarker);
            OnMarkerDeleted(this, (TextMarkerEventArgs)eventArgs);
            CommandUpdateTextMarkersExecute(_selectedEntries);        
        }


        public event EventHandler MarkerDeleted;
        public event EventHandler MarkerAdded;

        public void OnMarkerDeleted(object sender, TextMarkerEventArgs e)
        {
            if(MarkerDeleted != null)
            {
                MarkerDeleted(this, e);
            }
        }
    }
}
