﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using YalvLib.Common;
using YalvLib.Common.Interfaces;
using YalvLib.Model;
using YalvLib.Providers;
using YalvLib.Strings;
using YalvLib.ViewModel.Process;

namespace YalvLib.ViewModel
{
    /// <summary>
    /// Main ViewModel of Valv Lib control
    /// </summary>
    public class YalvViewModel : BindableObject
    {
        #region fields

        private readonly LogAnalysis _logAnalysis;
        private readonly DisplayLogViewModel _logEntryRows;
        private readonly ManageRepositoryViewModel _manageRepoViewModel;
        private readonly ManageTextMarkersViewModel _manageTextMarkersViewModel;

        private string _calculatedDelta;
        private LogFileLoader _fileLoader;

        #endregion fields

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

            _logEntryRows = new DisplayLogViewModel(_manageTextMarkersViewModel);

            CommandCancelProcessing = new CommandRelay(CommandCancelProcessing_Executed,
                                                       CommandCancelProcessing_CanExecute);

            CommandRefresh = new CommandRelay(CommandRefreshExecute, CommandRequiresDataCanExecute);
            CommandDelete = new CommandRelay(LogEntryRows.CommandDeleteExecute, LogEntryRows.CommandDeleteCanExecute);
            FilterYalvView = new CommandRelay(CommandFilterYalvView, CommandRequiresDataCanExecute);

            CommandUpdateTextMarkers = new CommandRelay(_manageTextMarkersViewModel.CommandUpdateTextMarkersExecute,
                                                        _manageTextMarkersViewModel.CommandUpdateTextMarkersCanExecute);

            CommandUpdateDelta = new CommandRelay(CommandUpdateDeltaExecute, CommandUpdateDeltaCanExecute);
        }

        private bool CommandCancelProcessing_CanExecute(object obj)
        {
            return (_fileLoader != null);
        }

        private object CommandCancelProcessing_Executed(object arg)
        {
            if (_fileLoader != null)
                _fileLoader.Cancel();

            return null;
        }

        #endregion constructor

        #region properties

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
                                     Resources.MainWindowVM_commandOpenFileExecute_XmlFilesCaption,
                                     Resources.MainWindowVM_commandOpenFileExecute_AllFilesCaption);
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
                                     Resources.MainWindowVM_commandOpenFileExecute_AllFilesCaption);
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
                                     Resources.MainWindowVM_commandOpenFileExecute_AllFilesCaption);
            }
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
            get { return _logEntryRows; }
        }

        /// <summary>
        /// Getter of the repositories managed view model
        /// </summary>
        public ManageRepositoryViewModel ManageRepositoriesViewModel
        {
            get { return _manageRepoViewModel; }
        }


        /// <summary>
        /// Get the Display Text Marker instance
        /// </summary>
        public ManageTextMarkersViewModel ManageTextMarkersViewModel
        {
            get { return _manageTextMarkersViewModel; }
        }


        /// <summary>
        /// Getter / Setter of the Calculated Delta between 2 entries
        /// </summary>
        public string CalculatedDelta
        {
            get { return _calculatedDelta; }
            private set
            {
                if (_calculatedDelta != value)
                {
                    _calculatedDelta = value;
                    NotifyPropertyChanged(() => CalculatedDelta);
                }
            }
        }

        /// <summary>
        /// Get whether there are data items in the collection or not
        /// (there may be no items to display if filter is applied but thats a different issue)
        /// </summary>
        public bool HasData
        {
            get { return (LogEntryRows != null && LogEntryRows.HasData); }
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


        /// <summary>
        /// UpdateTextMarkers Command
        /// </summary>
        public ICommandAncestor CommandUpdateTextMarkers { get; protected set; }

        /// <summary>
        /// ChangeTextmarkers Command
        /// </summary>
        public ICommandAncestor CommandChangeTextMarkers { get; protected set; }


        /// <summary>
        /// Update calculatedDelta command
        /// </summary>
        public ICommandAncestor CommandUpdateDelta { get; protected set; }

        public ICommandAncestor CommandCancelProcessing { get; protected set; }

        #endregion Command

        #endregion properties

        #region methods

        private void ManageRepoViewModelOnPropertyChanged(object sender,
                                                          PropertyChangedEventArgs propertyChangedEventArgs)
        {
            LoadFinishedEvent(true);
        }

        /// <summary>
        /// Load a log4net log file to display its content through this ViewModel.
        /// </summary>
        /// <param name="paths">file path</param>
        public void LoadFiles(List<string> paths)
        {
            if (_fileLoader != null)
            {
                if (MessageBox.Show(
                    "A load operation is currently in progress. Would you like to cancel the current process?",
                    "Load in progress...",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    _fileLoader.Cancel();
                }
            }

            _fileLoader = new LogFileLoader();
            _fileLoader.LoadResultEvent += fileLoader_loadResultEvent;
            ManageRepositoriesViewModel.IsLoading = true;

            _fileLoader.ExecuteAsynchronously(delegate()
                                                  {
                                                      ManageRepositoriesViewModel.LoadFiles(paths,
                                                                                            EntriesProviderType.Xml,
                                                                                            ManageRepositoriesViewModel);
                                                  },
                                     true);
        }

        /// <summary>
        /// Method is executed when the background process finishes and returns here
        /// because it was cancelled or is done processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileLoader_loadResultEvent(object sender, LogFileLoader.ResultEvent e)
        {
            _fileLoader.LoadResultEvent -= fileLoader_loadResultEvent;
            _fileLoader = null;
            ManageRepositoriesViewModel.IsLoading = false;
            if(e.InnerException != null)
            {
                ApplicationException exp = new ApplicationException(e.Message, e.InnerException)
                                               {Source = "LoadFileLoader"};
                exp.Data.Add("Process cancelled?", e.Cancel.ToString());
                MessageBox.Show(string.Format("Exception : {0} \n {1}", exp, e.Error), Resources.GlobalHelper_ParseLogFile_Error_Title,
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);
                LoadFinishedEvent(false);
            }
            else { LoadFinishedEvent(true); }
            
        }

        /// <summary>
        /// Load a database file
        /// </summary>
        /// <param name="path">file path</param>
        public void LoadSqliteDatabase(string path)
        {
            //ManageRepositoriesViewModel.LoadFiles(new List<string> {path}, EntriesProviderType.Sqlite);
            LoadFinishedEvent(true);
        }


        /// <summary>
        /// Load an entire LogAnalysis session
        /// </summary>
        /// <param name="path">file path</param>
        public void LoadLogAnalysisSession(string path)
        {
            _fileLoader = new LogFileLoader();
            _fileLoader.LoadResultEvent += fileLoader_loadResultEvent;
            ManageRepositoriesViewModel.IsLoading = true;
            _fileLoader.ExecuteAsynchronously(delegate()
                {
                ManageRepositoriesViewModel.LoadFiles(new List<string>(){path},
                                                      EntriesProviderType.Yalv,
                                                      ManageRepositoriesViewModel);
                },true);
        }

        /// <summary>
        /// Implementation of the Refresh command (reload data and apply filters)
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal virtual object CommandRefreshExecute(object parameter)
        {
            LogEntryRows.CommandRefreshExecute(LoadFinishedEvent);
            return null;
        }

        internal virtual bool CommandRequiresDataCanExecute(object parameter)
        {
            return HasData;
        }

        private bool CommandUpdateDeltaCanExecute(object obj)
        {
            return ((IEnumerable<LogEntryRowViewModel>) obj).Count() == 2;
        }

        private object CommandUpdateDeltaExecute(object arg)
        {
            var list = arg as IEnumerable<LogEntryRowViewModel>;
            if (list != null)
            {
                // We sure has 2 and only 2 entries
                List<LogEntryRowViewModel> logEntryRowViewModels = list as List<LogEntryRowViewModel> ?? list.ToList();
                LogEntryRowViewModel entry1 = logEntryRowViewModels.ElementAt(0);
                LogEntryRowViewModel entry2 = logEntryRowViewModels.ElementAt(1);
                CalculatedDelta = String.Concat("between GuId : ", entry1.Entry.GuId, " and GuId : ", entry2.Entry.GuId,
                                                " = ",
                                                GlobalHelper.GetTimeDelta(entry1.Entry.TimeStamp, entry2.Entry.TimeStamp));
            }
            return null;
        }

        /// <summary>
        /// Default command method for applying column filters.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual object CommandFilterYalvView(object parameter)
        {
            if (_logEntryRows != null)
            {
                _logEntryRows.ApplyFilter();
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
            if (loadWasSuccessful)
            {
                _fileLoader = new LogFileLoader();
                _fileLoader.LoadResultEvent += RefreshCommandsCanExecute;
                ManageRepositoriesViewModel.IsLoading = true;
                _fileLoader.ExecuteAsynchronously(delegate
                                                      {
                                                          LogEntryRows.SetEntries(
                                                              ManageRepositoriesViewModel.Repositories.ToList());
                                                      }, true);
            }
        }

        private void RefreshCommandsCanExecute(object sender, LogFileLoader.ResultEvent resultEvent)
        {
            _fileLoader.LoadResultEvent -= RefreshCommandsCanExecute;
            _fileLoader = null;
            ManageRepositoriesViewModel.IsLoading = false;
            CommandRefresh.CanExecute(null);
            CommandDelete.CanExecute(null);
            FilterYalvView.CanExecute(null);

            RaisePropertyChanged("HasData");
        }

        #endregion methods
    }
}