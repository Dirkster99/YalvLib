namespace YalvLib.ViewModel
{
    using log4netLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using YalvLib.Common;
    using YalvLib.Model;

    /// <summary>
    /// This class contains every TextMarkerViewModels linked to the selected entries
    /// </summary>
    public class ManageTextMarkersViewModel : BindableObject, IManageTextMarkersViewModel
    {
        private readonly ObservableCollection<TextMarkerViewModel> _textMarkerVmList;
        private List<ILogEntryRowViewModel> _selectedEntries;
        private TextMarkerViewModel _textMarkerAdd;
        private bool _displayOnlyCommonMarkers;
        private LogAnalysis _analysis;

        /// <summary>
        /// Constructor
        /// </summary>
        public ManageTextMarkersViewModel(LogAnalysis analysis)
        {
            _analysis = analysis;
            _textMarkerAdd = new TextMarkerViewModel(new TextMarker(new List<LogEntry>(), string.Empty, string.Empty));
            _textMarkerVmList = new ObservableCollection<TextMarkerViewModel>();
            _textMarkerAdd.CommandChangeTextMarker.Executed += ExecuteChange;
            _textMarkerAdd.TextMarkerDeleted += ExecuteCancel;
            CommandUpdateTextMarkers = new CommandRelay(CommandUpdateTextMarkersExecute,
                                                        CommandUpdateTextMarkersCanExecute);
        }


        /// <summary>
        /// Generate the markers for the model loaded in the workspace
        /// </summary>
        /// <param name="currentAnalysis"></param>
        private void GenerateMarkersFromAnalysis(LogAnalysis currentAnalysis)
        {
            foreach(var textMarker in currentAnalysis.TextMarkers)
            {
                _textMarkerVmList.Add(new TextMarkerViewModel(textMarker));
            }
        }

        /// <summary>
        /// getter / setter of the log analysis linked to the textmarkers
        /// </summary>
        public LogAnalysis Analysis
        {
            get { return _analysis; }
            set
            {
                if (value != null && value != Analysis)
                {
                    _analysis = value;
                    GenerateMarkersFromAnalysis(Analysis);
                }
            }
        }

        /// <summary>
        /// Get/Set for the TextmarkerViewModel row used
        /// to add a new TextMarker
        /// </summary>
        public TextMarkerViewModel TextMarkerToAdd
        {
            get { return _textMarkerAdd; }
            private set
            {
                _textMarkerAdd = value;
                NotifyPropertyChanged(() => TextMarkerToAdd);
            }
        }


        /// <summary>
        /// Getter for the TextMarkerViewModel list
        /// </summary>
        public ObservableCollection<TextMarkerViewModel> TextMarkerViewModels
        {
            get { return _textMarkerVmList; }
        }

        /// <summary>
        /// Getter / Setter for the list of entries
        /// selected by the user
        /// </summary>
        public List<ILogEntryRowViewModel> SelectedEntries
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
            set
            {
                _displayOnlyCommonMarkers = value;
                if (_selectedEntries != null)
                    CommandUpdateTextMarkersExecute(_selectedEntries);
            }
        }

        /// <summary>
        /// This command is used to Update the TextMarkers
        /// </summary>
        public ICommandAncestor CommandUpdateTextMarkers { get; protected set; }

        #region IManageTextMarkersViewModel Members

        /// <summary>
        /// Handler used when a textMarker is deleted
        /// </summary>
        public event EventHandler MarkerDeleted;

        /// <summary>
        /// Handler used when a TextMarker is added
        /// </summary>
        public event EventHandler MarkerAdded;

        #endregion

        /// <summary>
        /// Generate a list of TextMarkerViewModel 
        /// from the TextMarker list given
        /// </summary>
        /// <param name="textMarkerList">TextMarker list</param>
        public void GenerateViewModels(List<TextMarker> textMarkerList)
        {
            foreach (TextMarkerViewModel textMarkerViewModel in TextMarkerViewModels)
            {
                textMarkerViewModel.TextMarkerDeleted -= ExecuteCancel;
            }

            TextMarkerViewModels.Clear();
            GetNewTextMarkerToAdd();

            foreach (TextMarker textMarker in textMarkerList)
            {
                TextMarkerViewModels.Add(new TextMarkerViewModel(textMarker));
            }
            foreach (TextMarkerViewModel textMarkerViewModel in TextMarkerViewModels)
            {
                textMarkerViewModel.TextMarkerDeleted += ExecuteCancel;
            }
        }

        /// <summary>
        /// Update the list of TextMarkerViewModel
        /// from the selected entries and the displayOnlyCommonMarker value
        /// </summary>
        /// <param name="arg">List of selected log entries</param>
        /// <returns>null</returns>
        internal object CommandUpdateTextMarkersExecute(object arg)
        {
            _selectedEntries = new List<ILogEntryRowViewModel>((IEnumerable<ILogEntryRowViewModel>) arg);

            IEnumerable<TextMarker> markers =
                YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.GetTextMarkersForEntries(
                    _selectedEntries.Select(x => x.Entry));

            List<TextMarker> markersCommon = markers.Where(
                x =>
                _selectedEntries.All(
                    e =>
                    YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.GetTextMarkersForEntry(e.Entry).Contains(x))).
                ToList();

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
            return ((IEnumerable<ILogEntryRowViewModel>) obj).Any();
        }

        /// <summary>
        /// Execute the changes on the textmarkers, updating the textmarker quantity of the linked log entries
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        public void ExecuteChange(object sender, EventArgs e)
        {
            TextMarkerViewModels.Add(TextMarkerToAdd);
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.AddTextMarker(_selectedEntries.Select(x => x.Entry),
                                                                                TextMarkerToAdd.Marker);
            foreach (LogEntryRowViewModel entry in _selectedEntries)
            {
                entry.UpdateTextMarkerQuantity();
            }
            MarkerAdded(this, null);
            GetNewTextMarkerToAdd();
        }

        /// <summary>
        /// Execute the delete on the textMarker
        /// </summary>
        /// <param name="obj">object</param>
        /// <param name="eventArgs">args</param>
        public void ExecuteCancel(object obj, EventArgs eventArgs)
        {
            var args = eventArgs as TextMarkerEventArgs;
            if(args == null)
                throw new Exception("Args null");
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.DeleteTextMarker(args.TextMarker);
            OnMarkerDeleted(this, (TextMarkerEventArgs) eventArgs);
            CommandUpdateTextMarkersExecute(_selectedEntries);
        }


        /// <summary>
        /// Rise the event MarkerDeleted when a marker is deleted 
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Textmarker deleted</param>
        public void OnMarkerDeleted(object sender, TextMarkerEventArgs e)
        {
            if (MarkerDeleted != null)
            {
                MarkerDeleted(this, e);
            }
        }

        
    }
}