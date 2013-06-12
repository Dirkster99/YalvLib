using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using YalvLib.Common;
using YalvLib.Model;
using YalvLib.Strings;

namespace YalvLib.ViewModel
{
    /// <summary>
    /// This class represent a repository displayed on the list of files in the view
    /// </summary>
    public class RepositoryViewModel : BindableObject
    {
        private bool _isActive;
        private LogEntryRepository _repository;

        /// <summary>
        /// Constructor, taking a LogEntryRepository containing the source of the logentries
        /// </summary>
        /// <param name="repository"></param>
        public RepositoryViewModel(LogEntryRepository repository)
        {
            Repository = repository;
            Active = true;
            CommandRemoveRepository = new CommandRelay(ExecuteRemoveRepository, CanExecuteRemoveRepository);
        }

        /// <summary>
        /// Tells if the repository is active within the view ie tells if the data has to be displayed on the grid
        /// </summary>
        public bool Active
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    NotifyPropertyChanged(() => Active);
                    OnActiveChanged();
                }
            }
        }

        /// <summary>
        /// Getter of the file path
        /// </summary>
        public string Path
        {
            get { return _repository.Path; }
        }

        /// <summary>
        /// Getter of the filename
        /// </summary>
        public string PathDisplay
        {
            get { return Path.Split(new[] {'\\'}, StringSplitOptions.None).Last(); }
        }

        /// <summary>
        /// Getter / Setter of the linked Repository
        /// </summary>
        public LogEntryRepository Repository
        {
            get { return _repository; }
            private set
            {
                if (_repository != value)
                {
                    _repository = value;
                    NotifyPropertyChanged(() => Repository);
                }
            }
        }

        /// <summary>
        /// This command is used to tell the repo manager that this instance of repo has to be remove from the list
        /// </summary>
        public CommandRelay CommandRemoveRepository { get; private set; }

        /// <summary>
        /// Event RepositoryDeleted is raised to update the view
        /// </summary>
        public event EventHandler RepositoryDeleted;

        private bool CanExecuteRemoveRepository(object obj)
        {
            return true;
        }

        private object ExecuteRemoveRepository(object arg)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this repository ?",
                                                      Resources.
                                                          MarkerRow_DeleteConfirmation_Caption,
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                if (RepositoryDeleted != null)
                {
                    RepositoryDeleted(this, null);
                }
            }
            return null;
        }

        /// <summary>
        /// Active Changed is used to tell the repo manager that the state of this repo has changed
        /// </summary>
        public event PropertyChangedEventHandler ActiveChanged;

        /// <summary>
        /// When the active property has changed, we rise the propertychangedeventhandler
        /// </summary>
        public void OnActiveChanged()
        {
            if (ActiveChanged != null)
            {
                ActiveChanged(this, new PropertyChangedEventArgs("Active"));
            }
        }
    }
}