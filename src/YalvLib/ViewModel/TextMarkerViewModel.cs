using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using YalvLib.Common;
using YalvLib.Model;

namespace YalvLib.ViewModel
{
    public class TextMarkerViewModel : BindableObject
    {
        private TextMarker _marker;
        private string _author;
        private string _message;


        public TextMarkerViewModel(TextMarker tm)
        {
            _marker = tm;
            _author = _marker.Author;
            _message = _marker.Message;
            CommandChangeTextMarker = new CommandRelay(ExecuteChangeTextMarker, CanExecuteChangeTextmarker);
            CommandCancelTextMarker = new CommandRelay(ExecuteCancelTextMarker, CanExecuteCancelTextMarker);

            CanExecuteCancel = CommandCancelTextMarker.CanExecute(null);
            CanExecuteChange = CommandChangeTextMarker.CanExecute(null);
        }

        public TextMarker Marker
        {
            get { return _marker; }
        }

        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    RaisePropertyChanged("Author");
                }
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    RaisePropertyChanged("Message");
                }
            }
        }

        private bool _canExecuteCancel;
        public bool CanExecuteCancel
        {
            get { return _canExecuteCancel; } 
            set
            {
                if (_canExecuteCancel != value)
                {
                    _canExecuteCancel = value;
                    RaisePropertyChanged("CanExecuteCancel");
                }                
            }
        }

        private bool _canExecuteChange;
        public bool CanExecuteChange
        {
            get { return _canExecuteChange; }
            set
            {
                if (_canExecuteChange != value)
                {
                    _canExecuteChange = value;
                    RaisePropertyChanged("CanExecuteChange");
                }
            }
        }


        public CommandRelay CommandCancelTextMarker { get; private set; }

        private bool CanExecuteCancelTextMarker(object obj)
        {
            return _marker.LogEntryCount() < 1;
        }

        public object ExecuteCancelTextMarker(object obj)
        {
            if(_marker.LogEntryCount() >= 1)
            {
                _marker.RemoveEntry(_marker.LogEntries);
            }
            return null;
        }



        public CommandRelay CommandChangeTextMarker { get; private set; }

        private bool CanExecuteChangeTextmarker(object obj)
        {
            if( _message != string.Empty 
                && _author != string.Empty
                && _author != null
                && _message != null
                && _marker.LogEntryCount() >= 1)
                return true;
            return false;
        }

        public object ExecuteChangeTextMarker(object o)
        {               
            _marker.Author = _author;
            _marker.Message = _message;
            return _marker;
        }

        private object ExecuteChangeTextMarker()
        {
            _marker.Author = _author;
            _marker.Message = _message;
            return _marker;
        }

        
    }
}