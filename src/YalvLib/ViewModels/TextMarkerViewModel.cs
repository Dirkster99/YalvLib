namespace YalvLib.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using YalvLib.Common;
    using YalvLib.Model;
    using log4netLib.Strings;

    /// <summary>
    /// View Model of a TextMarker, it binds the action on the views to functions
    /// </summary>
    public class TextMarkerViewModel : BindableObject, IEditableObject,
                                                       ICloneable
    {
        #region fields
        private string _author;
        private bool _canExecuteCancel;
        private bool _canExecuteChange;
        private TextMarker _marker;
        private string _message;
        private bool _isInEditMode;

        private TextMarkerViewModel _cachedCopy;
        #endregion fields

        #region ctors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tm">TextMarker linked to the viewModel</param>
        public TextMarkerViewModel(TextMarker tm)
        {
            Marker = tm;

////            CommandChangeTextMarker = new CommandRelay(ExecuteChangeTextMarker, CanExecuteChangeTextmarker);
////            CommandCancelTextMarker = new CommandRelay(ExecuteCancelTextMarker, CanExecuteCancelTextMarker);
////            PropertyChanged += TextMarkerViewModelPropertyChanged;

            Author = _marker.Author;
            Message = _marker.Message;
        }
        #endregion ctors

////        /// <summary>
////        /// Event handler for a deleted marker
////        /// </summary>
////        public event EventHandler<TextMarkerEventArgs> TextMarkerDeleted;

        #region properties
        /// <summary>
        /// Gets Marker model of this viewmodel
        /// </summary>
        public TextMarker Marker
        {
            get { return _marker; }
            protected set { _marker = value; }
        }

        /// <summary>
        /// Get/Set author
        /// If the value is changed, we rise a propertychanged event
        /// </summary>
        public string Author
        {
            get { return _author; }
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
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyPropertyChanged(() => Message);
                }
            }
        }

        /// <summary>
        /// Return a string informing about the number of entries linked to the marker
        /// </summary>
        public string LinkedEntries
        {
            get { 
                if (_marker.LogEntryCount() > 1) 
                    return string.Format("{0} entries linked", _marker.LogEntryCount());
                return string.Empty;
            }
        }

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
        public bool IsMultipleTextMarker
        {
            get
            {
                return _marker.LogEntryCount() > 1 &&
                       YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.IsMultiMarker(_marker);
            }
        }

        /// <summary>
        /// Gets whether the item is currently in edit mode or not.
        /// </summary>
        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            protected set
            {
                if (_isInEditMode != value)
                {
                    _isInEditMode = value;
                    NotifyPropertyChanged(() => this.IsInEditMode);
                }
            }
        }
        #endregion properties

        #region methods
        #region IEditableObject support
        /// <summary>
        /// Standard property that is called when this item is changing
        /// its mode from non-editing (just display data) to editing.
        /// </summary>
        void IEditableObject.BeginEdit()
        {
            // save object state before entering edit mode
            _cachedCopy = this.Clone() as TextMarkerViewModel;

            // ensure edit mode flag is set
            IsInEditMode = true;
        }

        void IEditableObject.CancelEdit()
        {
            // restore original object state
            if (_cachedCopy != null)
                CopyItem(_cachedCopy, this);

            // clear cached data
            _cachedCopy = null;

            // ensure edit mode flag is unset
            IsInEditMode = false;
        }

        void IEditableObject.EndEdit()
        {
            // clear cached data
            _cachedCopy = null;

            Marker.Author = Author;
            Marker.Message = Message;
            Marker.DateLastModification = DateTime.Now;

            IsInEditMode = false;    // ensure edit mode flag is unset
        }
        #endregion

        /// <summary>
        /// Gets a copy of this object to implement <see cref="ICloneable"/>.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var copy = new TextMarkerViewModel(Marker);
            CopyItem(this, copy);

            return copy;
        }

        private void CopyItem(TextMarkerViewModel sourceItem,
                              TextMarkerViewModel targetItem,
                              bool CopyIsInEditMode = false)
        {
            targetItem.Author = sourceItem.Author;
            targetItem.Message = sourceItem.Message;

            targetItem.Marker.Author = sourceItem.Marker.Author;
            targetItem.Marker.Message = sourceItem.Marker.Message;
            targetItem.Marker.DateLastModification = sourceItem.Marker.DateLastModification;

            if (CopyIsInEditMode == true)
                targetItem.IsInEditMode = sourceItem.IsInEditMode;
        }

////        /// <summary>
////        /// This function is called everytime a property is changed on the view
////        /// </summary>
////        private void TextMarkerViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
////        {
////            EvaluateCanExecuteConditions();
////        }

////        private void EvaluateCanExecuteConditions()
////        {
////            CanExecuteCancel = CommandCancelTextMarker.CanExecute(null);
////            CanExecuteChange = CommandChangeTextMarker.CanExecute(null);
////        }

////        private bool CanExecuteCancelTextMarker(object obj)
////        {
////            return Author != string.Empty && Message != string.Empty;
////        }
////
////        /// <summary>
////        /// If the marker is linked to some entries, we delete them.
////        /// </summary>
////        /// <param name="obj"></param>
////        /// <returns></returns>
////        private object ExecuteCancelTextMarker(object obj)
////        {
////            if (_marker.LogEntryCount() >= 1)
////            {
////                if (_marker.LogEntryCount() > 1)
////                    if (MessageBox.Show(Resources.MarkerRow_DeleteConfirmation,
////                                        Resources.
////                                            MarkerRow_DeleteConfirmation_Caption,
////                                        MessageBoxButton.YesNo,
////                                        MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
////                        return null;
////                OnExecutedCancelTextMarker(new TextMarkerEventArgs(_marker));
////            }
////            Author = string.Empty;
////            Message = string.Empty;
////            return null;
////        }
////
////        private void OnExecutedCancelTextMarker(TextMarkerEventArgs textMarkerEventArgs)
////        {
////            if (TextMarkerDeleted != null)
////            {
////                TextMarkerDeleted(this, textMarkerEventArgs);
////            }
////        }

////        private bool CanExecuteChangeTextmarker(object obj)
////        {
////            return _message != string.Empty
////                   && _author != string.Empty
////                   && _author != null
////                   && _message != null;
////        }
////
////        /// <summary>
////        /// Apply the modifications to the marker linked to this instance of textmarkerviewModel
////        /// </summary>
////        /// <param name="o"></param>
////        /// <returns></returns>
////        private object ExecuteChangeTextMarker(object o)
////        {
////            _marker.Author = _author;
////            _marker.Message = _message;
////            _marker.DateLastModification = DateTime.Now;
////            return null;
////        }
        #endregion
    }
}