using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YalvLib.Common;
using YalvLib.Model;

namespace YalvLib.ViewModel
{
    public class RepositoryViewModel : BindableObject
    {

        private LogEntryRepository _repository;
        private bool _isActive;

        public RepositoryViewModel(LogEntryRepository repository)
        {
            Repository = repository;
            Active = true;

        }

        public bool Active
        {
            get { return _isActive; }
            set { 
                if(_isActive != value)
                {
                    _isActive = value;
                    NotifyPropertyChanged(() => Active);
                    OnActiveChanged();
                }
            }
        }

        public string Path
        {
            get { return _repository.Path; }
        }

        public string PathDisplay
        {
            get { return Path.Split(new char[] {'\\'}, StringSplitOptions.None).Last(); }
        }

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

        public event PropertyChangedEventHandler ActiveChanged;
        public void OnActiveChanged()
        {
            if (ActiveChanged != null)
            {
                ActiveChanged(this, new PropertyChangedEventArgs("Active"));
            }
        }  
    }
}
