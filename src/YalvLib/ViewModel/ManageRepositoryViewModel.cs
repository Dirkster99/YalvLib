using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YalvLib.Infrastructure.Sqlite;
using YalvLib.Model;
using YalvLib.Providers;
using YalvLib.Strings;

namespace YalvLib.ViewModel
{
    using System;
    using System.IO;
    using System.Windows;
    using YalvLib.Common;

    /// <summary>
    /// Class to manage data for a logfile
    /// </summary>
    public class ManageRepositoryViewModel : BindableObject
    {
        #region fields

        private bool mIsLoading;

        #endregion fields

        private ObservableCollection<RepositoryViewModel> _repositories;
        private bool mIsFileLoaded;

        #region Constructors

        /// <Summary>
        /// Standard constructor of the <seealso cref="ManageRepositoryViewModel"/> class
        /// </Summary>
        public ManageRepositoryViewModel()
        {
            this._repositories = new ObservableCollection<RepositoryViewModel>();
            this.mIsFileLoaded = false;
            this.mIsLoading = false;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get/set property to indicate whether a logfile is currently being parsed and loaded or not.
        /// </summary>
        public bool IsLoading
        {
            get { return this.mIsLoading; }

            set
            {
                if (this.mIsLoading != value)
                {
                    this.mIsLoading = value;
                    NotifyPropertyChanged(() => IsLoading);
                }
            }
        }

        /// <summary>
        /// Get whether a log file has been loaded or not
        /// </summary>
        public bool IsFileLoaded
        {
            get { return this.mIsFileLoaded; }

            internal set
            {
                if (this.mIsFileLoaded != value)
                {
                    this.mIsFileLoaded = value;
                    NotifyPropertyChanged(() => IsFileLoaded);
                }
            }
        }


        public EntriesProviderType ProviderType { get; set; }


        /// <summary>
        /// Get a the file system path of the log file
        /// </summary>
        public ObservableCollection<RepositoryViewModel> Repositories
        {
            get { return (this._repositories ?? new ObservableCollection<RepositoryViewModel>()); }

            internal set
            {
                if (this._repositories != value)
                {
                    this._repositories = value;
                    NotifyPropertyChanged(() => Repositories);
                }
            }
        }


        public bool HasData
        {
            get{ return Repositories.Any();}
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
                foreach (var logEntryRepository in listFileRepos)
                {
                    Repositories.Add(new RepositoryViewModel(logEntryRepository));
                    Repositories.Last().ActiveChanged += OnActiveChanged;
                }
                NotifyPropertyChanged(() => Repositories);
            }
        }

        private void OnActiveChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            NotifyActiveChanged(propertyChangedEventArgs.PropertyName);
        }

        public event PropertyChangedEventHandler ActiveChanged;

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
                    var loadedWorkspace = new LogAnalysisWorkspaceLoader(paths.ElementAt(0)).Load();
                    var listRepo = CheckRepositoriesValidity(loadedWorkspace);

                    if(listRepo.Any())
                    {
                        var repoListString = new StringBuilder();
                        foreach (var repo in listRepo) repoListString.AppendLine(repo.Path);
                        MessageBox.Show("These repositories are already loaded : " + repoListString);
                        if(listRepo.Count == YalvRegistry.Instance.ActualWorkspace.SourceRepositories.Count)
                        {
                            MessageBox.Show("Nothing to be loaded");
                            return;
                        }
                    }

                    loadedWorkspace.SourceRepositories = loadedWorkspace.SourceRepositories.Except(listRepo).ToList();
                    YalvRegistry.Instance.SetActualLogAnalysisWorkspace(loadedWorkspace);
                    YalvRegistry.Instance.ActualWorkspace.currentAnalysis =
                        YalvRegistry.Instance.ActualWorkspace.Analyses.First();
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
                            MessageBox.Show(string.Format("Cannot access file '{0}'.", path));
                            return;
                        }
                        // If this is the first file or the file hasnt be loaded yet, we can add it to the repo
                        if ((Repositories.Any() && !reposPath.Contains(path)) || !Repositories.Any())
                        {
                            listRepo.Add(CreateLogFileEntryRepository(path));
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
            List<LogEntryRepository> invalidRepos = new List<LogEntryRepository>();
            foreach(var loadedRepo in loadedWorkspace.SourceRepositories)
            {
                if (YalvRegistry.Instance.ActualWorkspace.SourceRepositories.Contains(loadedRepo))
                    invalidRepos.Add(loadedRepo);
            }
            return invalidRepos;
        }

        private void UpdateWorkspace(List<LogEntryRepository> repositories)
        {
            AddRepositories(repositories);
            foreach (var logEntryRepository in repositories)
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
            if (this.Repositories.Count == 0 && this.IsFileLoaded == true)
            {
                if (MessageBox.Show(
                    YalvLib.Strings.Resources.MainWindowVM_commandDeleteExecute_DeleteCheckedFiles_ConfirmText,
                    YalvLib.Strings.Resources.MainWindowVM_commandDeleteExecute_DeleteCheckedFiles_ConfirmTitle,
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    return;

                // Delete all selected file
                if (this.DeleteFiles(Repositories) == true)
                {
                    this.Repositories = new ObservableCollection<RepositoryViewModel>();
                    this.IsFileLoaded = false;
                }
            }

            return;
        }

        internal virtual bool CommandDeleteCanExecute()
        {
            return (this.Repositories.Count == 0 && this.IsFileLoaded == true);
        }

        /// <summary>
        /// Physically delete a file in the file system.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private bool DeleteFiles(ObservableCollection<RepositoryViewModel> repos)
        {
            try
            {
                foreach (var repo in repos)
                {
                    FileInfo fileInfo = new FileInfo(repo.Repository.Path);
                    if (fileInfo != null)
                        fileInfo.Delete();
                }


                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(YalvLib.Strings.Resources.MainWindowVM_deleteFile_ErrorMessage_Text, repos, ex.Message),
                    YalvLib.Strings.Resources.MainWindowVM_deleteFile_ErrorMessage_Title, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }

        #endregion commandDelete

        #endregion Methods
    }
}
