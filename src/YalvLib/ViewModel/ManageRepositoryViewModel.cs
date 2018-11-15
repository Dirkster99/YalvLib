namespace YalvLib.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using YalvLib.Common;
    using YalvLib.Infrastructure.Sqlite;
    using YalvLib.Model;
    using YalvLib.Providers;
    using log4netLib.Strings;

    /// <summary>
    /// Class to manage data for every logfile loaded
    /// </summary>
    public class ManageRepositoryViewModel : BindableObject
    {
        #region fields
        private bool _isLoading;
        private bool _isFileLoaded;
        private readonly ObservableCollection<RepositoryViewModel> _repositories;
        #endregion fields

        #region Constructors
        /// <Summary>
        /// Standard constructor of the <seealso cref="ManageRepositoryViewModel"/> class
        /// </Summary>
        public ManageRepositoryViewModel()
        {
            _isFileLoaded = false;
            _isLoading = false;

            _repositories = new ObservableCollection<RepositoryViewModel>();
            foreach (var repo in YalvRegistry.Instance.ActualWorkspace.SourceRepositories)
            {
                _repositories.Add(new RepositoryViewModel(repo));
            }
        }
        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get/set property to indicate whether a logfile is currently being parsed and loaded or not.
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyPropertyChanged(() => IsLoading);
                }
            }
        }

        /// <summary>
        /// Get whether a log file has been loaded or not
        /// </summary>
        public bool IsFileLoaded
        {
            get { return _isFileLoaded; }
            internal set
            {
                if (_isFileLoaded != value)
                {
                    _isFileLoaded = value;
                    NotifyPropertyChanged(() => IsFileLoaded);
                }
            }
        }

        /// <summary>
        /// Gets the provider type.
        /// Used to be able to read log data from different data sources
        /// (eg.: files or DB Server database etc ...)
        /// </summary>
        public EntriesProviderType ProviderType { get; protected set; }

        /// <summary>
        /// Get a the file system path of the log file
        /// </summary>
        public ObservableCollection<RepositoryViewModel> Repositories
        {
            get { return (_repositories ?? new ObservableCollection<RepositoryViewModel>()); }
        }

        /// <summary>
        /// Getter that tells if the list of repositories contains any logentryrepository
        /// </summary>
        public bool HasData
        {
            get { return Repositories.Any(); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Add the list of repositories to the current instance of ManageRepositoryViewModel
        /// </summary>
        /// <param name="listFileRepos">list of repositories</param>
        public void AddRepositories(List<LogEntryRepository> listFileRepos)
        {
            if (listFileRepos != null)
            {
                foreach (LogEntryRepository logEntryRepository in listFileRepos)
                {
                    Repositories.Add(new RepositoryViewModel(logEntryRepository));
                    Repositories.Last().ActiveChanged += OnActiveChanged;
                    Repositories.Last().RepositoryDeleted += OnRepositoryDeleted;
                }
                NotifyPropertyChanged(() => Repositories);
            }
        }

        private void OnRepositoryDeleted(object sender, EventArgs e)
        {
            var repo = sender as RepositoryViewModel;
            if (repo != null)
            {
                //If we delete a repository we have to remove the textmarkers linked to the entries contained in this repository
                foreach (LogEntry entry in repo.Repository.LogEntries)
                {
                    foreach (TextMarker textmarker in YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.TextMarkers)
                    {
                        if (textmarker.LogEntries.Contains(entry))
                        {
                            textmarker.LogEntries.Remove(entry);
                            if (!textmarker.LogEntries.Any())
                                YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.TextMarkers.Remove(textmarker);
                        }
                    }
                }
                YalvRegistry.Instance.ActualWorkspace.SourceRepositories.Remove(repo.Repository);
                Repositories.Remove(repo);
                ActiveChanged(this, null); // The active didnt changed but it just refresh the view
            }
        }

        private void OnActiveChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            NotifyActiveChanged(propertyChangedEventArgs.PropertyName);
        }

        /// <summary>
        /// This handler is used to tell the LogVIewModel when a repository active property has changed
        /// </summary>
        public event PropertyChangedEventHandler ActiveChanged;

        /// <summary>
        /// This function notify the LogViewModel that the active state of one repository has changed
        /// </summary>
        /// <param name="propertyName">name of the property (Active)</param>
        public void NotifyActiveChanged(string propertyName)
        {
            if (ActiveChanged != null)
            {
                ActiveChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        /* public void LoadFiles(List<string> paths, EntriesProviderType providerType)
         {
             try
             {
                 ProviderType = providerType;
                 if (ProviderType.Equals(EntriesProviderType.Yalv))
                 {
                     LogAnalysisWorkspace loadedWorkspace = new LogAnalysisWorkspaceLoader(paths.ElementAt(0)).Load();

                     YalvRegistry.Instance.SetActualLogAnalysisWorkspace(loadedWorkspace);

                     YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis =
                         YalvRegistry.Instance.ActualWorkspace.Analyses.First();

                     _repositories.Clear();

                     AddRepositories(YalvRegistry.Instance.ActualWorkspace.SourceRepositories.ToList());
                 }
                 else
                 {
                     var listRepo = new List<LogEntryRepository>();
                     List<string> reposPath =
                         Repositories.Select(x => x.Repository.Path).ToList();

                     foreach (string path in paths)
                     {
                         if (!File.Exists(path))
                         {
                             MessageBox.Show(Resources.GlobalHelper_CantAccessFile_Error_Text, path);
                             return;
                         }

                         // If this is the first file or the file hasnt be loaded yet, we can add it to the repo
                         if ((Repositories.Any() && !reposPath.Contains(path)) || !Repositories.Any())
                         {
                             listRepo.Add(CreateLogFileEntryRepository(path));
                         }
                         else
                         {
                             MessageBox.Show(Resources.GlobalHelper_RepositoryAlreadyExists_Error_Text);
                         }
                     }
                     foreach (LogEntryRepository logEntryRepository in listRepo)
                     {
                         YalvRegistry.Instance.ActualWorkspace.SourceRepositories.Add(logEntryRepository);
                     }
                 }
             }
             catch (Exception exception)
             {
                 string message = string.Format(Resources.GlobalHelper_ParseLogFile_Error_Text, paths,
                                                exception.Message);
                 MessageBox.Show(message, Resources.GlobalHelper_ParseLogFile_Error_Title,
                                 MessageBoxButton.OK, MessageBoxImage.Exclamation);
             }
         }*/

        /// <summary>
        /// Load the list of files with the given provider type.
        /// Update the repository list 
        /// </summary>
        /// <param name="paths">Paths of the files</param>
        /// <param name="providerType">Type of the provider for the given files</param>
        /// <param name="vm"></param>
        public void LoadFiles(List<string> paths,
                                     EntriesProviderType providerType,
                                     ManageRepositoryViewModel vm)
        {
            var cancelTokenSource = new CancellationTokenSource();
            var cancelToken = cancelTokenSource.Token;
            try
            {
                vm.ProviderType = providerType;
                if (vm.ProviderType.Equals(EntriesProviderType.Yalv))
                {
                    LogAnalysisWorkspace loadedWorkspace = new LogAnalysisWorkspaceLoader(paths.ElementAt(0)).Load();

                    YalvRegistry.Instance.ActualWorkspace = (loadedWorkspace);

                    cancelToken.ThrowIfCancellationRequested();

                    YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis =
                        YalvRegistry.Instance.ActualWorkspace.Analyses.First();

                    cancelToken.ThrowIfCancellationRequested();


                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (Action)delegate
                                    {
                                        vm._repositories.Clear();

                                        vm.AddRepositories(
                                            YalvRegistry.Instance.ActualWorkspace.SourceRepositories.ToList());
                                    });
                }
                else
                {
                    var listRepo = new List<LogEntryRepository>();
                    List<string> reposPath = vm.Repositories.Select(x => x.Repository.Path).ToList();

                    foreach (string path in paths)
                    {
                        cancelToken.ThrowIfCancellationRequested();

                        if (!File.Exists(path))
                        {
                            MessageBox.Show(Resources.GlobalHelper_CantAccessFile_Error_Text, path);
                            return;
                        }

                        // If this is the first file or the file hasnt be loaded yet, we can add it to the repo
                        if ((vm.Repositories.Any() && !reposPath.Contains(path)) || !vm.Repositories.Any())
                        {
                            listRepo.Add(vm.CreateLogFileEntryRepository(path, ProviderType));
                        }
                        else
                        {
                            MessageBox.Show(Resources.GlobalHelper_RepositoryAlreadyExists_Error_Text);
                        }
                    }
                    vm.UpdateWorkspace(listRepo);
                }
            }
            catch (Exception exception)
            {
                string message = string.Format(Resources.GlobalHelper_ParseLogFile_Error_Text, paths,
                                               exception.Message);
                /*MessageBox.Show(message, Resources.GlobalHelper_ParseLogFile_Error_Title,
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);*/
                throw new Exception(message, exception);
            }
        }


        private void UpdateWorkspace(List<LogEntryRepository> repositories)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                (Action)delegate { AddRepositories(repositories); });

            foreach (LogEntryRepository logEntryRepository in repositories)
            {
                YalvRegistry.Instance.ActualWorkspace.SourceRepositories.Add(logEntryRepository);
            }
        }

        /// <summary>
        /// Creates a log entry model object that can hold common log4net
        /// imput information as well as specialied information that is
        /// applicable to a given data source (eg. file or DB server) only.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="providerType"></param>
        /// <returns></returns>
        private LogEntryRepository CreateLogFileEntryRepository(string path,
                                                                EntriesProviderType providerType)
        {
            switch (providerType)
            {
                case EntriesProviderType.Sqlite:
                    return new LogEntrySqliteRepository(path);

                case EntriesProviderType.Xml:
                    return new LogEntryFileRepository(path);
            }

            return null;
        }

        #region commandDelete

        internal virtual void CommandDeleteExecute()
        {
            if (Repositories.Count == 0 && IsFileLoaded)
            {
                if (MessageBox.Show(
                    Resources.MainWindowVM_commandDeleteExecute_DeleteCheckedFiles_ConfirmText,
                    Resources.MainWindowVM_commandDeleteExecute_DeleteCheckedFiles_ConfirmTitle,
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    return;

                // Delete all selected file
                if (DeleteFiles(Repositories))
                {
                    Repositories.Clear();
                    IsFileLoaded = false;
                }
            }
        }

        internal virtual bool CommandDeleteCanExecute()
        {
            return (Repositories.Count == 0 && IsFileLoaded);
        }

        /// <summary>
        /// Physically delete a file in the file system.
        /// </summary>
        /// <param name="repos">repos to delete</param>
        /// <returns></returns>
        private bool DeleteFiles(ObservableCollection<RepositoryViewModel> repos)
        {
            try
            {
                foreach (RepositoryViewModel repo in repos)
                {
                    var fileInfo = new FileInfo(repo.Repository.Path);
                    fileInfo.Delete();
                }


                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Resources.MainWindowVM_deleteFile_ErrorMessage_Text, repos, ex.Message),
                    Resources.MainWindowVM_deleteFile_ErrorMessage_Title, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }

        #endregion commandDelete

        #endregion Methods
    }
}