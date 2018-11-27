namespace YalvLib.ViewModels.Markers
{
    using System;
    using System.ComponentModel;
    using YalvLib.Common;
    using YalvLib.Model;
    using static YalvLib.ViewModels.Markers.MarkerEditEventArgs;

    /// <summary>
    /// View Model of a TextMarker, it binds the action on the views to functions
    /// </summary>
    public class TextMarkerViewModel : BindableObject, IEditableObject,
                                                       ICloneable
    {
        #region fields
        private bool _canExecuteCancel;
        private bool _canExecuteChange;
        private bool _isInEditMode;

        private TextMarkerViewModel _cachedCopy;
        private TextMarker _Marker;
        #endregion fields

        #region ctors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tm">TextMarker linked to the viewModel</param>
        public TextMarkerViewModel(TextMarker tm)
        {
            _Marker = tm;
        }
        #endregion ctors

        internal event MarkerEditEventHandler MarkerEditEventArgs;

        #region properties
        /// <summary>
        /// Gets Marker model of this viewmodel
        /// </summary>
        protected TextMarker Marker { get { return _Marker; } }

        /// <summary>
        /// Get/Set author
        /// If the value is changed, we rise a propertychanged event
        /// </summary>
        public string Author
        {
            get
            {
                if (Marker != null)
                    return Marker.Author;

                return string.Empty;
            }

            set
            {
                if (Marker != null)
                {
                    if (Marker.Author != value)
                    {
                        Marker.Author = value;
                        NotifyPropertyChanged(() => Author);
                    }
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
                if (Marker != null)
                    return Marker.Message;

                return string.Empty;
            }
            set
            {
                if (Marker != null)
                {
                    if (Marker.Message != value)
                    {
                        Marker.Message = value;
                        NotifyPropertyChanged(() => Message);
                    }
                }
            }
        }

        /// <summary>
        /// Return a string informing about the number of entries linked to the marker
        /// </summary>
        public string LinkedEntries
        {
            get
            {
                if (Marker != null)
                {
                    if (Marker.LogEntryCount() > 1)
                        return string.Format("{0} entries linked", Marker.LogEntryCount());
                }

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
                if (Marker == null)
                    return false;

                return Marker.LogEntryCount() > 1 &&
                       YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.IsMultiMarker(Marker);
            }
        }

        /// <summary>
        /// Gets the date and time at which this item was created.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                if (Marker == null)
                    return default(DateTime);

                return Marker.DateCreation;
            }
        }

        /// <summary>
        /// Gets the date and time at which this item was modified last.
        /// </summary>
        public DateTime DateModified
        {
            get
            {
                if (Marker == null)
                    return default(DateTime);

                return Marker.DateLastModification;
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
        /// Standard method that is called when this item is changing
        /// its mode from non-editing (just display data) to editing.
        /// </summary>
        void IEditableObject.BeginEdit()
        {
            // save object state before entering edit mode
            _cachedCopy = this.Clone() as TextMarkerViewModel;

            // ensure edit mode flag is set
            IsInEditMode = true;

            if (MarkerEditEventArgs != null)
                MarkerEditEventArgs(this, new Markers.MarkerEditEventArgs(Marker, EditEvent.BeginEdit));
        }

        /// <summary>
        /// Standard method that is called when editing of an item is cancelled.
        /// The object should roll back any changes and exit editing mode.
        /// </summary>
        void IEditableObject.CancelEdit()
        {
            // restore original object state
            if (_cachedCopy != null)
                CopyItem(_cachedCopy, this);

            // clear cached data
            _cachedCopy = null;

            // ensure edit mode flag is unset
            IsInEditMode = false;

            if (MarkerEditEventArgs != null)
                MarkerEditEventArgs(this, new Markers.MarkerEditEventArgs(Marker, EditEvent.CancelEdit));
        }

        /// <summary>
        /// Standard method that is called when editing of an item ends succesfully.
        /// The object should permanently store changes and exit editing mode.
        /// </summary>
        void IEditableObject.EndEdit()
        {
            // clear cached data
            _cachedCopy = null;              // Destroy unedited back copy and
            IsInEditMode = false;           // ensure edit mode flag is unset

            if (MarkerEditEventArgs != null)
                MarkerEditEventArgs(this, new Markers.MarkerEditEventArgs(Marker, EditEvent.CommitEdit));

            NotifyPropertyChanged(() => DateModified);
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

        /// <summary>
        /// Returns a COPY of the model associated with this viewmodel object.
        /// </summary>
        /// <returns></returns>
        internal TextMarker GetModel()
        {
            return _Marker.Clone() as TextMarker;
        }

        private void CopyItem(TextMarkerViewModel sourceItem,
                              TextMarkerViewModel targetItem,
                              bool CopyIsInEditMode = false)
        {
            targetItem._Marker = sourceItem.Marker.Clone() as TextMarker;

            if (CopyIsInEditMode == true)
                targetItem.IsInEditMode = sourceItem.IsInEditMode;
        }
        #endregion
    }
}