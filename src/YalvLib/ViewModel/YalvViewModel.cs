using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using YalvLib.Domain;
using YalvLib.Model;
using YalvLib.Providers;

namespace YalvLib.ViewModel
{
    using YalvLib.Common;
    using YalvLib.Common.Interfaces;

    /// <summary>
    /// Main ViewModel of Valv Lib control
    /// </summary>
    public class YalvViewModel : BindableObject
    {
        #region fields

        private DisplayLogViewModel _logEntryRows = null;
        private ManageTextMarkersViewModel _manageTextMarkersViewModel = null;
        private ManageRepositoryViewModel _manageRepoViewModel = null;
 

        private LogAnalysis _logAnalysis = null;

        #endregion fields

        /// <summary>
        /// Defines files available extensions
        /// </summary>
        public static string FileExtensionDialogFilter
        {
            get
            {
                return string.Format("*.log4j,*.log,*.txt,*.xml|*.log4j;*.log;*.txt;*.xml" +
                                     "|{0} (*.log4j)|*.log4j" +
                                     "|{1} (*.log)|*.log" +
                                     "|{2} (*.txt)|*.txt" +
                                     "|{3} (*.xml)|*.xml" +
                                     "|{4} (*.*)|*.*", "Log4j files",
                                     "Log files",
                                     "Text files",
                                     YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_XmlFilesCaption,
                                     YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_AllFilesCaption);
            }
        }


        /// <summary>
        /// Defines databases available extensions
        /// </summary>
        public static string DatabaseExtensionDialogFilter
        {
            get
            {
                return string.Format("{0}| *.db*" +
                                     "|{1} (*.*)|*.*", "Database files",
                                     YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_AllFilesCaption);
            }
        }

        /// <summary>
        /// Defines yalv files extension
        /// </summary>
        public static string LogAnalysisExtensionDialogFilter
        {
            get
            {
                return string.Format("{0}| *.yalv*" +
                                     "|{1} (*.*)|*.*", "LogAnalysis files",
                                     YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_AllFilesCaption);
            }
        }

        #region constructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public YalvViewModel()
        {
            _manageTextMarkersViewModel = new ManageTextMarkersViewModel();
            _manageRepoViewModel = new ManageRepositoryViewModel();
            _manageRepoViewModel.ActiveChanged += ManageRepoViewModelOnPropertyChanged;

            _logAnalysis = new LogAnalysis();
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis = _logAnalysis;

            this._logEntryRows = new DisplayLogViewModel(_manageTextMarkersViewModel);
            

            this.CommandRefresh = new CommandRelay(this.CommandRefreshExecute, this.CommandRequiresDataCanExecute);
            this.CommandDelete = new CommandRelay(this.LogEntryRows.CommandDeleteExecute,
                                                  this.LogEntryRows.CommandDeleteCanExecute);
            this.FilterYalvView = new CommandRelay(this.CommandFilterYalvView, this.CommandRequiresDataCanExecute);
            CommandUpdateTextMarkers = new CommandRelay(_manageTextMarkersViewModel.CommandUpdateTextMarkersExecute, _manageTextMarkersViewModel.CommandUpdateTextMarkersCanExecute);

        }

        private void ManageRepoViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            LoadFinishedEvent(true);
        }

        #endregion constructor

        #region properties

        /// <summary>
        /// Get property to determine the name of
        /// the currently viewed log4net file or
        /// empty string if there is no file being viewed at present.
        /// </summary>
        public List<string> FilePaths
        {
            get
            {
                var filePaths = new List<string>();
                if (ManageRepositoriesViewModel != null)
                {
                    filePaths.AddRange(ManageRepositoriesViewModel.Repositories.Select(repo => repo.Repository.Path));
                }
                return filePaths;
            }
        }

        /// <summary>
        /// Get a list of filter columns and data items (for display in a DataGridView)
        /// </summary>
        public DisplayLogViewModel LogEntryRows
        {
            get { return this._logEntryRows; }
        }

        public ManageRepositoryViewModel ManageRepositoriesViewModel
        {
            get { return _manageRepoViewModel; }
        }


        /// <summary>
        /// Get the Display Text Marker instance
        /// </summary>
        public ManageTextMarkersViewModel ManageTextMarkersViewModel
        {
            get
            {
                return _manageTextMarkersViewModel;
            }
        }

        /// <summary>
        /// Get whether there are data items in the collection or not
        /// (there may be no items to display if filter is applied but thats a different issue)
        /// </summary>
        public bool HasData
        {
            get { return (this.LogEntryRows != null && this.LogEntryRows.HasData); }
        }

        #region Command

        /// <summary>
        /// FilterYalvView command to switches a filtered view on or off
        /// </summary>
        public ICommandAncestor FilterYalvView { get; protected set; }

        /// <summary>
        /// Refresh Command
        /// </summary>
        public ICommandAncestor CommandRefresh { get; protected set; }

        /// <summary>
        /// Delete Command
        /// </summary>
        public ICommandAncestor CommandDelete { get; protected set; }

        public ICommandAncestor CommandUpdateTextMarkers { get; protected set; }

        public ICommandAncestor CommandChangeTextMarkers { get; protected set; }

        #endregion Command

        #endregion properties

        #region methods



        /// <summary>
        /// Load a log4nez log file to display its content through this ViewModel.
        /// </summary>
        /// <param name="paths">file path</param>
        public void LoadFiles(List<string> paths)
        {
            ManageRepositoriesViewModel.LoadFiles(paths, EntriesProviderType.Xml);
            LoadFinishedEvent(true);
        }


        /// <summary>
        /// Load a database file
        /// </summary>
        /// <param name="path">file path</param>
        public void LoadSqliteDatabase(string path)
        {
            ManageRepositoriesViewModel.LoadFiles(new List<string>() { path }, EntriesProviderType.Sqlite);
            LoadFinishedEvent(true);
        }


        /// <summary>
        /// Load an entire LogAnalysis session
        /// </summary>
        /// <param name="path">file path</param>
        public void LoadLogAnalysisSession(string path)
        {
            ManageRepositoriesViewModel.LoadFiles(new List<string>() { path }, EntriesProviderType.Yalv);
            LoadFinishedEvent(true);
        }


        /// <summary>
        /// Implementation of the Refresh command (reload data and apply filters)
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal virtual object CommandRefreshExecute(object parameter)
        {
            this.LogEntryRows.CommandRefreshExecute(this.LoadFinishedEvent);

            return null;
        }

        internal virtual bool CommandRequiresDataCanExecute(object parameter)
        {
            return this.HasData;
        }


        /// <summary>
        /// Default command method for applying column filters.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual object CommandFilterYalvView(object parameter)
        {
            if (this._logEntryRows != null)
            {
                this._logEntryRows.ApplyFilter();
            }

            return null;
        }

        /// <summary>
        /// This is a callback method that is always called when
        /// the internal load process is finished
        /// (even when it failed to finish after initialization).
        /// </summary>
        /// <param name="loadWasSuccessful"></param>
        private void LoadFinishedEvent(bool loadWasSuccessful)
        {
            this.LogEntryRows.SetEntries(ManageRepositoriesViewModel.Repositories.ToList());
            this.RefreshCommandsCanExecute();
        }

        private void RefreshCommandsCanExecute()
        {
            this.CommandRefresh.CanExecute(null);
            this.CommandDelete.CanExecute(null);
            this.FilterYalvView.CanExecute(null);

            this.RaisePropertyChanged("HasData");
        }

        #endregion methods
    }
}
