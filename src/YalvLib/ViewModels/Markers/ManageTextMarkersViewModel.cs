namespace YalvLib.ViewModels.Markers
{
    using log4netLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using YalvLib.Common;
    using YalvLib.Model;

    /// <summary>
    /// This class contains every TextMarkerViewModels linked to the selected entries
    /// </summary>
    public class ManageTextMarkersViewModel : BindableObject, IManageTextMarkersViewModel
    {
        #region fields
        private readonly ObservableCollection<TextMarkerViewModel> _textMarkerVmList;
        private List<ILogEntryRowViewModel> _selectedEntries;
        private bool _displayOnlyCommonMarkers;
        private LogAnalysis _analysis;

        private ICommand _AddTextMarker;
        private ICommand _DeleteTextMarker;
        private string _Author = "<current user>";
        #endregion fields

        #region ctors
        /// <summary>
        /// Constructor
        /// </summary>
        public ManageTextMarkersViewModel(LogAnalysis analysis)
        {
            _analysis = analysis;
            _textMarkerVmList = new ObservableCollection<TextMarkerViewModel>();
            CommandUpdateTextMarkers = new CommandRelay(CommandUpdateTextMarkersExecute,
                                                        CommandUpdateTextMarkersCanExecute);
        }
        #endregion ctors

        #region IManageTextMarkersViewModel Event Members
        /// <summary>
        /// Handler used when a textMarker is deleted
        /// </summary>
        public event EventHandler MarkerDeleted;

        /// <summary>
        /// Handler used when a TextMarker is added
        /// </summary>
        public event EventHandler MarkerAdded;
        #endregion IManageTextMarkersViewModel Event Members

        #region properties
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
        /// Gets/sets the author string currently used for text marking
        /// </summary>
        public string Author
        {
            get
            {
                if (_Author == null)
                    return string.Empty;

                return _Author;
            }

            set
            {
                if (_Author != value)
                {
                    _Author = value;
                    NotifyPropertyChanged(() => Author);
                }
            }
        }

        /// <summary>
        /// Getter for the TextMarkerViewModel list
        /// </summary>
        public IEnumerable<TextMarkerViewModel> TextMarkerViewModels
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

        /// <summary>
        /// Gets a command that adds a new textmarker to the currently selected items.
        /// </summary>
        public ICommand AddTextMarkerCommand
        {
            get
            {
                if (_AddTextMarker == null)
                    _AddTextMarker = new RelayCommand<object>
                        ((p) => AddTextMarkerCommand_Executed(p),
                         (p) => AddTextMarkerCommand_CanExecute(p));

                return _AddTextMarker;
            }
        }

        /// <summary>
        /// Gets a command to remove a textmarker from the collection
        /// of available textmarkers in the currently selected item.
        /// 
        /// The <see cref="TextMarkerViewModel"/> item to be removed is
        /// expected as a parameter of the command.
        /// </summary>
        public ICommand DeleteTextMarkerCommand
        {
            get
            {
                if (_DeleteTextMarker == null)
                    _DeleteTextMarker = new RelayCommand<object>
                        ((p) => DeleteTextMarkerCommand_Executed(p),
                         (p) => DeleteTextMarkerCommand_CanExecute(p));

                return _DeleteTextMarker;
            }
        }

        /// <summary>
        /// Gets the number of textmarkers that are current available.
        /// </summary>
        public int TextMarkerViewModels_Count { get { return _textMarkerVmList.Count; } }
        #endregion properties

        #region methods
        /// <summary>
        /// Generate a list of TextMarkerViewModel 
        /// from the TextMarker list given
        /// </summary>
        /// <param name="textMarkerList">TextMarker list</param>
        public void GenerateViewModels(List<TextMarker> textMarkerList)
        {
            RemoveAllMarkers();

            foreach (TextMarker textMarker in textMarkerList)
                AddMarker(new TextMarkerViewModel(textMarker));
        }

        #region Add Delete TextMarker Command
        internal object AddTextMarkerCommand_Executed(object arg)
        {
            var model = new TextMarker(new List<LogEntry>(),
                                       this.Author, "<Double Click me to comment>");

            var newTxMarker = new TextMarkerViewModel(model);
            this.AddMarker(newTxMarker);

            // Link text marker to currently selected items via TextMarker (model) Entries
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.AddTextMarker(
                _selectedEntries.Select(x => x.Entry), newTxMarker.GetModel());

            foreach (LogEntryRowViewModel entry in _selectedEntries)
            {
                entry.UpdateTextMarkerQuantity();
            }

            OnMarkerAdded(new TextMarkerEventArgs(model)); // Generate an event on this to refresh the view

            return null;
        }

        internal bool AddTextMarkerCommand_CanExecute(object obj)
        {
            if (_selectedEntries == null)
                return false;

            if (_selectedEntries.Count <= 0)
                return false;

            return true;
        }

        private object DeleteTextMarkerCommand_Executed(object arg)
        {
            var param = arg as TextMarkerViewModel;

            if (param == null)
                return null;

            this.RemoveMarker(param);

            var marker = param.GetModel();
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.DeleteTextMarker(marker);
            OnMarkerDeleted(new TextMarkerEventArgs(marker));
            CommandUpdateTextMarkersExecute(_selectedEntries);

            return null;
        }

        private bool DeleteTextMarkerCommand_CanExecute(object arg)
        {
            var param = arg as TextMarkerViewModel;

            if (param == null)
                return false;

            return true;
        }

        private void AddMarker(TextMarkerViewModel newMarker)
        {
            newMarker.MarkerEditEventArgs += NewMarker_MarkerEditEventArgs;
            _textMarkerVmList.Add(newMarker);
        }

        private void RemoveMarker(TextMarkerViewModel oldMarker)
        {
            oldMarker.MarkerEditEventArgs -= NewMarker_MarkerEditEventArgs;
            _textMarkerVmList.Remove(oldMarker);
        }

        private void RemoveAllMarkers()
        {
            for (int i = _textMarkerVmList.Count; i > 0; i--)
                RemoveMarker(_textMarkerVmList[i-1]);
        }

        private void NewMarker_MarkerEditEventArgs(object sender, MarkerEditEventArgs e)
        {
            if (e != null)
            {
                // Update the author to be the last person who edited this entry
                if (e.EventType == MarkerEditEventArgs.EditEvent.CommitEdit)
                {
                    var vm = sender as TextMarkerViewModel;
                    vm.Author = this.Author;
                }
            }
        }
        #endregion Add Delete TextMarker Command

        /// <summary>
        /// Raise the event MarkerDeleted when a marker is deleted 
        /// </summary>
        /// <param name="e">Textmarker deleted</param>
        private void OnMarkerDeleted(TextMarkerEventArgs e)
        {
            if (MarkerDeleted != null)
                MarkerDeleted(this, e); // Generate an event on this to refresh the view
        }

        /// <summary>
        /// Raise the event MarkerDeleted when a marker is deleted 
        /// </summary>
        /// <param name="e">Textmarker deleted</param>
        private void OnMarkerAdded(TextMarkerEventArgs e)
        {
            if (MarkerDeleted != null)
                MarkerAdded(this, e); // Generate an event on this to refresh the view
        }

        /// <summary>
        /// Update the list of TextMarkerViewModel
        /// from the selected entries and the displayOnlyCommonMarker value
        /// </summary>
        /// <param name="arg">List of selected log entries</param>
        /// <returns>null</returns>
        internal object CommandUpdateTextMarkersExecute(object arg)
        {
            if (TextMarkerViewModels.Count() > 0 && _textMarkerVmList.Count > 0)
            {
                foreach (LogEntryRowViewModel entry in _selectedEntries)
                {
                    entry.UpdateTextMarkerQuantity();
                }
            }

            _selectedEntries = new List<ILogEntryRowViewModel>((IEnumerable<ILogEntryRowViewModel>)arg);

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

        internal bool CommandUpdateTextMarkersCanExecute(object obj)
        {
            return ((IEnumerable<ILogEntryRowViewModel>)obj).Any();
        }

        /// <summary>
        /// Generate the markers for the model loaded in the workspace
        /// </summary>
        /// <param name="currentAnalysis"></param>
        private void GenerateMarkersFromAnalysis(LogAnalysis currentAnalysis)
        {
            foreach (var textMarker in currentAnalysis.TextMarkers)
            {
                this.AddMarker(new TextMarkerViewModel(textMarker));
            }
        }
        #endregion methods
    }
}