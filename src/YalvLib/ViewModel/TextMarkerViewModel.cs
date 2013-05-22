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
    /// <summary>
    /// View Model of a TextMarker, it binds the action on the views to functions
    /// </summary>
    public class TextMarkerViewModel : BindableObject
    {
        private TextMarker _marker;
        private string _author;
        private string _message;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tm">TextMarker linked to the viewModel</param>
        public TextMarkerViewModel(TextMarker tm)
        {
            _marker = tm; 

            CommandChangeTextMarker = new CommandRelay(ExecuteChangeTextMarker, CanExecuteChangeTextmarker);
            CommandCancelTextMarker = new CommandRelay(ExecuteCancelTextMarker, CanExecuteCancelTextMarker);
            PropertyChanged += TextMarkerViewModel_PropertyChanged;

            Author = _marker.Author;
            Message = _marker.Message;
        }
        /// <summary>
        /// This function is called everytime a property is changed on the view
        /// </summary>
        private void TextMarkerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EvaluateCanExecuteConditions();
        }

        private void EvaluateCanExecuteConditions()
        {
            if(CanExecuteCancel != CommandCancelTextMarker.CanExecute(null))
            { CommandCancelTextMarker.OnCanExecuteChanged(); }
            CanExecuteCancel = CommandCancelTextMarker.CanExecute(null);

            if (CanExecuteChange != CommandChangeTextMarker.CanExecute(null))
            { CommandChangeTextMarker.OnCanExecuteChanged(); }
            CanExecuteChange = CommandChangeTextMarker.CanExecute(null);
        }

        /// <summary>
        /// Getter Marker
        /// </summary>
        public TextMarker Marker
        {
            get { return _marker; }
        }


        /// <summary>
        /// Get/Set author
        /// If the value is changed, we rise a propertychanged event
        /// </summary>
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

        /// <summary>
        /// Get/Set message
        /// If the value is changedm we rise a propertychanged event
        /// </summary>
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
        /// <summary>
        /// If we can execute the cancel, we rise a property changed event
        /// </summary>
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
        /// <summary>
        /// If we can execute the change, we rise a property changed event
        /// </summary>
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
            return _marker.LogEntryCount() <= 1;
        }

        /// <summary>
        /// If the marker is linked to some entries, we delete them.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object ExecuteCancelTextMarker(object obj)
        {
            Author = string.Empty;
            Message = string.Empty;
            if(_marker.LogEntryCount() >= 1)
            {
                YalvRegistry.Instance.ActualWorkspace.Analysis.DeleteTextMarker(_marker);
            }
            return null;
        }



        public CommandRelay CommandChangeTextMarker { get; private set; }

        private bool CanExecuteChangeTextmarker(object obj)
        {
            return _message != string.Empty 
                   && _author != string.Empty
                   && _author != null
                   && _message != null;
        }

        public object ExecuteChangeTextMarker(object o)
        {               
            _marker.Author = _author;
            _marker.Message = _message;
            if(!YalvRegistry.Instance.ActualWorkspace.Analysis.TextMarkers.Contains(Marker))
                YalvRegistry.Instance.ActualWorkspace.Analysis.Markers.Add(Marker);
            return _marker;
        }

        
    }
}