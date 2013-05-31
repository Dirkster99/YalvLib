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
            CanExecuteCancel = CommandCancelTextMarker.CanExecute(null);
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
                    NotifyPropertyChanged(() => Author);
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
                    this.NotifyPropertyChanged(() => this.Message);
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
                    NotifyPropertyChanged(() => CanExecuteCancel);
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
                    NotifyPropertyChanged(() => CanExecuteChange);
                }
            }
        }

        /// <summary>
        /// Determine if the the marker is linked to several log entries
        /// </summary>
        public bool isMultipleTextMarker
        {
            get
            {
                return _marker.LogEntryCount() > 1 && YalvRegistry.Instance.ActualWorkspace.Analysis.IsMultiMarker(_marker);
            }
        }


        public CommandRelay CommandCancelTextMarker { get; private set; }

        private bool CanExecuteCancelTextMarker(object obj)
        {
            return Author != string.Empty && Message != string.Empty;
        }

        /// <summary>
        /// If the marker is linked to some entries, we delete them.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object ExecuteCancelTextMarker(object obj)
        {
            if(_marker.LogEntryCount() >= 1)
            {
                if(_marker.LogEntryCount() > 1)
                    if (MessageBox.Show(Strings.Resources.MarkerRow_DeleteConfirmation,
                                                              Strings.Resources.
                                                                  MarkerRow_DeleteConfirmation_Caption,
                                                              MessageBoxButton.YesNo,
                                                              MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                        return null;
                YalvRegistry.Instance.ActualWorkspace.Analysis.DeleteTextMarker(_marker);
            }
            Author = string.Empty;
            Message = string.Empty;
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
            return null;
        }
    }
}