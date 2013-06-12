using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YalvLib.Common;
using YalvLib.Infrastructure.Sqlite;
using YalvLib.Model;
using YalvLib.Providers;
using YalvLib.Strings;

namespace YalvLib.ViewModel
{
    /// <summary>
    /// Class to manage data for every logfile loaded
    /// </summary>
    public class ManageRepositoryViewModel : BindableObject
    {
        #region fields

        private bool _isLoading;

        #endregion fields

        private ObservableCollection<RepositoryViewModel> _repositories;
        private bool _isFileLoaded;

        #region Constructors

        /// <Summary>
        /// Standard constructor of the <seealso cref="ManageRepositoryViewModel"/> class
        /// </Summary>
        public ManageRepositoryViewModel()
        {
            _repositories = new ObservableCollection<RepositoryViewModel>();
            _isFileLoaded = false;
            _isLoading = false;
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
        /// Get / Set the provider type. Used to be able to read different files
        /// </summary>
        public EntriesProviderType ProviderType { get; set; }


        /// <summary>
        /// Get a the file system path of the log file
        /// </summary>
        public ObservableCollection<RepositoryViewModel> Repositories
        {
            get { return (_repositories ?? new ObservableCollection<RepositoryViewModel>()); }

            internal set
            {
                if (_repositories != value)
                {
                    _repositories = value;
                    NotifyPropertyChanged(() => Repositories);
                }
            }
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
            Repositories.Remove(repo);
            ActiveChanged(this, null);
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


        /// <summary>
        /// Load the list of files with the given provider type.
        /// Update the repository list 
        /// </summary>
        /// <param name="paths">Paths of the files</param>
        /// <param name="providerType">Type of the provider for the given files</param>
        public void LoadFiles(List<string> paths, EntriesProviderType providerType)
        {
            try
            {
                ProviderType = providerType;
                if (ProviderType.Equals(EntriesProviderType.Yalv))
                {
                    LogAnalysisWorkspace loadedWorkspace = new LogAnalysisWorkspaceLoader(paths.ElementAt(0)).Load();

                    YalvRegistry.Instance.SetActualLogAnalysisWorkspace(loadedWorkspace);

                    YalvRegistry.Instance.ActualWorkspace.currentAnalysis =
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
                        }else
                        {
                            MessageBox.Show(Resources.GlobalHelper_RepositoryAlreadyExists_Error_Text);
                        }
                    }
                    UpdateWorkspace(listRepo);
                }
            }
            catch (Exception exception)
            {
                string message = string.Format(Resources.GlobalHelper_ParseLogFile_Error_Text, paths,
                                               exception.Message);
                MessageBox.Show(message, Resources.GlobalHelper_ParseLogFile_Error_Title,
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        private List<LogEntryRepository> CheckRepositoriesValidity(LogAnalysisWorkspace loadedWorkspace)
        {
            var invalidRepos = new List<LogEntryRepository>();
            foreach (LogEntryRepository loadedRepo in loadedWorkspace.SourceRepositories)
            {
                if (YalvRegistry.Instance.ActualWorkspace.SourceRepositories.Contains(loadedRepo))
                    invalidRepos.Add(loadedRepo);
            }
            return invalidRepos;
        }

        private void UpdateWorkspace(List<LogEntryRepository> repositories)
        {
            AddRepositories(repositories);
            foreach (LogEntryRepository logEntryRepository in repositories)
            {
                YalvRegistry.Instance.ActualWorkspace.SourceRepositories.Add(logEntryRepository);
            }
        }

        private LogEntryRepository CreateLogFileEntryRepository(string path)
        {
            switch (ProviderType)
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
                    Repositories = new ObservableCollection<RepositoryViewModel>();
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