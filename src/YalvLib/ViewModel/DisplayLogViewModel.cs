using System.Threading.Tasks;
using System.Windows.Threading;

namespace YalvLib.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Input;
    using YalvLib.Common;
    using YalvLib.Common.Interfaces;
    using YalvLib.Model;
    using YalvLib.Strings;

    /// <summary>
    /// ViewModel class to organize all items relevant to a loaded logfile display
    /// (columns displayed, file name etc).
    /// </summary>
    public class DisplayLogViewModel : BindableObject
    {
        #region fields

        private const string PROP_FileDir = "FileDir";
        private const string PROP_LogView = "LogView";
        private const string PROP_HasData = "HasData";
        private const string PROP_Items = "LogEntryRowViewModels";
        private const string PROP_SelectedLogItem = "SelectedLogItem";
        private const string PROP_GoToLogItemId = "GoToLogItemId";
        private const string PROP_ShowLevelDebug = "ShowLevelDebug";
        private const string PROP_ShowLevelInfo = "ShowLevelInfo";
        private const string PROP_ShowLevelWarn = "ShowLevelWarn";
        private const string PROP_ShowLevelError = "ShowLevelError";
        private const string PROP_ShowLevelFatal = "ShowLevelFatal";
        private const string PROP_SelectAll = "SelectAll";
        private const string PROP_SelectDebug = "SelectDebug";
        private const string PROP_SelectInfo = "SelectInfo";
        private const string PROP_SelectWarn = "SelectWarn";
        private const string PROP_SelectError = "SelectError";
        private const string PROP_SelectFatal = "SelectFatal";
        private const string PROP_ItemsDebugCount = "ItemsDebugCount";
        private const string PROP_ItemsInfoCount = "ItemsInfoCount";
        private const string PROP_ItemsWarnCount = "ItemsWarnCount";
        private const string PROP_ItemsErrorCount = "ItemsErrorCount";
        private const string PROP_ItemsFatalCount = "ItemsFatalCount";
        private const string PROP_ItemsDebugFilterCount = "ItemsDebugFilterCount";
        private const string PROP_ItemsInfoFilterCount = "ItemsInfoFilterCount";
        private const string PROP_ItemsWarnFilterCount = "ItemsWarnFilterCount";
        private const string PROP_ItemsErrorFilterCount = "ItemsErrorFilterCount";
        private const string PROP_ItemsFatalFilterCount = "ItemsFatalFilterCount";
        private const string PROP_ItemsFilterCount = "ItemsFilterCount";


        private readonly ColumnsViewModel mDataGridColumns;

        private ObservableCollection<LogEntryRowViewModel> _rowViewModels;
        private ObservableCollection<LogEntryRowViewModel> _filtredRowViewModels; 
        private EvaluateLoadResult loadResultCallback = null;
        private FilterConverterViewModel _filterViewModel;

        private string mGoToLogItemId;

        private bool mIsFiltered;

        private int mItemsDebugCount;

        private int mItemsDebugFilterCount;
        private int mItemsErrorCount;
        private int mItemsErrorFilterCount;
        private int mItemsFatalCount;
        private int mItemsFatalFilterCount;
        private int mItemsFilterCount;
        private int mItemsInfoCount;
        private int mItemsInfoFilterCount;
        private int mItemsWarnCount;
        private int mItemsWarnFilterCount;
        private bool mSelectAll;
        private bool mSelectDebug;
        private bool mSelectError;
        private bool mSelectFatal;
        private bool mSelectInfo;
        private bool mSelectWarn;
        private LogEntryRowViewModel mSelectedLogItem;
        private bool mShowLevelDebug;
        private bool mShowLevelError;
        private bool mShowLevelFatal;
        private bool mShowLevelInfo;
        private bool mShowLevelWarn;

        public event EventHandler SelectedItemChanged;

        #endregion fields

        #region Constructor

        /// <summary>
        /// Class constructor
        /// </summary>
        public DisplayLogViewModel(IManageTextMarkersViewModel interfaceTextMarkerViewModel)
        {
            CommandClearFilters = new CommandRelay(CommandClearFiltersExecute,
                                                   CommandClearFilterCanExecute);

            CommandResetFilter = new CommandRelay(CommandResetFilterExecute, CommandResetFilterCanExecute);
            CommandApplyFilter = new CommandRelay(CommandApplyFilterExecute, CommandApplyFilterCanExecute);

            SelectAll = true;
            IsFiltered = false;
            LogEntryRowViewModels = new ObservableCollection<LogEntryRowViewModel>();
            RebuildLogView(LogEntryRowViewModels);
            _filterViewModel = new FilterConverterViewModel(YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis);
            _filterViewModel.PropertyChanged += (sender, args) => UpdateFilteredCounters(LogView);

            interfaceTextMarkerViewModel.MarkerDeleted +=
                (sender, args) => OnMarkerDeleteExecuted(sender, (TextMarkerEventArgs) args);
            interfaceTextMarkerViewModel.MarkerAdded += (sender, args) => UpdateCounters();

            // Default constructor contains column definitions
            // The callback is invocked when a column filter string item is changed
            // so we know that we should update the viewmodel filter
            mDataGridColumns = new ColumnsViewModel(ColumnsVmUpdateColumnFilter);
        }


        /// <summary>
        /// When a marker has been deletedm we update the textmarker quantity of the linked log entries
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public void OnMarkerDeleteExecuted(object obj, TextMarkerEventArgs e)
        {
            TextMarker marker = e.TextMarker;
            foreach (LogEntryRowViewModel row in _rowViewModels)
            {
                if (marker.LogEntries.Contains(row.Entry))
                {
                    row.UpdateTextMarkerQuantity();
                }
            }
            RefreshView();
        }

        #endregion Constructor

        #region delegate

        /// <summary>
        /// Declare a type of method to be called upon succesful load of log file.
        /// </summary>
        /// <param name="loadWasSuccessful"></param>
        public delegate void EvaluateLoadResult(bool loadWasSuccessful);

        #endregion delegate

        #region Properties

        /// <summary>
        /// Clear Command
        /// </summary>
        public ICommandAncestor CommandClearFilters { get; protected set; }

        /// <summary>
        /// Get a list of columns to be displayed in a DataGrid view display
        /// </summary>
        public ColumnsViewModel DataGridColumns
        {
            get { return mDataGridColumns; }
        }

        /// <summary>
        /// Getter of the FilterViewModel
        /// </summary>
        public FilterConverterViewModel FilterViewModel
        {
            get { return _filterViewModel; }
        }

        /// <summary>
        /// This property represents the datagrid viewmodel part and enables sorting and filtering
        /// being implemented in the viewmodel class.
        /// </summary>
        public CollectionView LogView { get; private set; }

        /// <summary>
        /// Get fewer there are data items in the collection or not
        /// (there may be no items to display if filter is applied but thats a different issue)
        /// </summary>
        internal bool HasData
        {
            get { return (_rowViewModels != null && (_rowViewModels.Count != 0)); }
        }

        /// <summary>
        /// LogItems property which is the main list of logitems
        /// (this property is bound to a view via CollectionView property)
        /// </summary>
        public ObservableCollection<LogEntryRowViewModel> LogEntryRowViewModels
        {
            get { return _rowViewModels; }

            set { _rowViewModels = value; }
        }

        public ObservableCollection<LogEntryRowViewModel> FiltredLogEntryRowViewModels
        {
            get { return _filtredRowViewModels; }

            set { _filtredRowViewModels = value; }
        }

        #region LogProperties

        /// <summary>
        /// SelectedLogItem Property
        /// </summary>
        public LogEntryRowViewModel SelectedLogItem
        {
            get { return mSelectedLogItem; }

            set
            {
                mSelectedLogItem = value;
                RaisePropertyChanged(PROP_SelectedLogItem);
                OnSelectedItemChanged(EventArgs.Empty);


                GoToLogItemId = mSelectedLogItem != null
                                    ? mSelectedLogItem.LogEntryId.ToString(CultureInfo.InvariantCulture)
                                    : string.Empty;
                RaisePropertyChanged(PROP_GoToLogItemId);
            }
        }

        /// <summary>
        /// GoToLogItemId Property
        /// </summary>
        public string GoToLogItemId
        {
            get { return mGoToLogItemId; }

            set
            {
                mGoToLogItemId = value;

                int idGoTo;
                int.TryParse(value, out idGoTo);
                UInt32 currentId = SelectedLogItem != null ? SelectedLogItem.LogEntryId : 0;

                if (idGoTo > 0 && idGoTo != currentId)
                {
                    var selectItem = (from it in LogEntryRowViewModels
                                      where it.LogEntryId == idGoTo
                                      select it).FirstOrDefault<LogEntryRowViewModel>();

                    if (selectItem != null)
                        SelectedLogItem = selectItem;
                }
                else
                    mGoToLogItemId = currentId != 0 ? currentId.ToString(CultureInfo.InvariantCulture) : string.Empty;

                RaisePropertyChanged(PROP_GoToLogItemId);
            }
        }

        protected void OnSelectedItemChanged(EventArgs e)
        {
            EventHandler handler = SelectedItemChanged;
            if (handler != null)
                handler(this, e);
        }

        #endregion LogProperties

        #region LogFilterProperties

        /// <summary>
        /// Get/set property to determine whether CollectionView is
        /// is filtered by a column's content or not.
        /// </summary>
        public bool IsFiltered
        {
            get { return mIsFiltered; }

            set
            {
                if (mIsFiltered != value)
                {
                    mIsFiltered = value;
                    RaisePropertyChanged("IsFiltered");
                    RefreshView();
                    CommandClearFilters.CanExecute(null);
                }
            }
        }

        /// <summary>
        /// ShowLevelDebug Property
        /// </summary>
        public bool ShowLevelDebug
        {
            get { return mShowLevelDebug; }

            set
            {
                if (value != mShowLevelDebug)
                {
                    mShowLevelDebug = value;
                    RaisePropertyChanged(PROP_ShowLevelDebug);
                    ResetLevelSelection();
                    RefreshView();
                }
            }
        }

        /// <summary>
        /// ShowLevelInfo Property
        /// </summary>
        public bool ShowLevelInfo
        {
            get { return mShowLevelInfo; }

            set
            {
                if (value != mShowLevelInfo)
                {
                    mShowLevelInfo = value;
                    RaisePropertyChanged(PROP_ShowLevelInfo);
                    ResetLevelSelection();
                    RefreshView();
                }
            }
        }

        /// <summary>
        /// ShowLevelWarn Property
        /// </summary>
        public bool ShowLevelWarn
        {
            get { return mShowLevelWarn; }

            set
            {
                if (value != mShowLevelWarn)
                {
                    mShowLevelWarn = value;
                    RaisePropertyChanged(PROP_ShowLevelWarn);
                    ResetLevelSelection();
                    RefreshView();
                }
            }
        }

        /// <summary>
        /// ShowLevelError Property
        /// </summary>
        public bool ShowLevelError
        {
            get { return mShowLevelError; }

            set
            {
                if (value != mShowLevelError)
                {
                    mShowLevelError = value;
                    RaisePropertyChanged(PROP_ShowLevelError);
                    ResetLevelSelection();
                    RefreshView();
                }
            }
        }

        /// <summary>
        /// ShowLevelFatal Property
        /// </summary>
        public bool ShowLevelFatal
        {
            get { return mShowLevelFatal; }

            set
            {
                if (value != mShowLevelFatal)
                {
                    mShowLevelFatal = value;
                    RaisePropertyChanged(PROP_ShowLevelFatal);
                    ResetLevelSelection();
                    RefreshView();
                }
            }
        }

        #endregion LogFilterProperties

        #region LogSelectionProperties

        /// <summary>
        /// SelectAll Property
        /// </summary>
        public bool SelectAll
        {
            get { return mSelectAll; }

            set
            {
                if (value != mSelectAll)
                {
                    mSelectAll = value;
                    RaisePropertyChanged(PROP_SelectAll);

                    if (mSelectAll)
                    {
                        mShowLevelDebug =
                            mShowLevelInfo =
                            mShowLevelWarn = mShowLevelError = mShowLevelFatal = true;
                        RefreshCheckBoxBinding();
                        RefreshView();
                    }

                    CommandClearFilters.CanExecute(null);
                }
            }
        }

        /// <summary>
        /// SelectDebug Property
        /// </summary>
        public bool SelectDebug
        {
            get { return mSelectDebug; }

            set
            {
                if (value != mSelectDebug)
                {
                    mSelectDebug = value;
                    RaisePropertyChanged(PROP_SelectDebug);

                    if (mSelectDebug)
                    {
                        mSelectAll =
                            mShowLevelInfo =
                            mShowLevelWarn = mShowLevelError = mShowLevelFatal = false;
                        mShowLevelDebug = true;
                        RefreshCheckBoxBinding();
                        RefreshView();

                        CommandClearFilters.CanExecute(null);
                    }
                }
            }
        }

        /// <summary>
        /// SelectInfo Property
        /// </summary>
        public bool SelectInfo
        {
            get { return mSelectInfo; }

            set
            {
                if (value != mSelectInfo)
                {
                    mSelectInfo = value;
                    RaisePropertyChanged(PROP_SelectInfo);

                    if (mSelectInfo)
                    {
                        mSelectAll =
                            mShowLevelDebug =
                            mShowLevelWarn = mShowLevelError = mShowLevelFatal = false;
                        mShowLevelInfo = true;
                        RefreshCheckBoxBinding();
                        RefreshView();

                        CommandClearFilters.CanExecute(null);
                    }
                }
            }
        }

        /// <summary>
        /// SelectWarn Property
        /// </summary>
        public bool SelectWarn
        {
            get { return mSelectWarn; }

            set
            {
                if (value != mSelectWarn)
                {
                    mSelectWarn = value;
                    RaisePropertyChanged(PROP_SelectWarn);

                    if (mSelectWarn)
                    {
                        mSelectAll =
                            mShowLevelDebug =
                            mShowLevelInfo = mShowLevelError = mShowLevelFatal = false;
                        mShowLevelWarn = true;
                        RefreshCheckBoxBinding();
                    }

                    RefreshView();

                    CommandClearFilters.CanExecute(null);
                }
            }
        }

        /// <summary>
        /// SelectError Property
        /// </summary>
        public bool SelectError
        {
            get { return mSelectError; }

            set
            {
                if (value != mSelectError)
                {
                    mSelectError = value;
                    RaisePropertyChanged(PROP_SelectError);

                    if (mSelectError)
                    {
                        mSelectAll =
                            mShowLevelDebug =
                            mShowLevelInfo = mShowLevelWarn = mShowLevelFatal = false;
                        mShowLevelError = true;
                        RefreshCheckBoxBinding();
                        RefreshView();
                    }

                    CommandClearFilters.CanExecute(null);
                }
            }
        }

        /// <summary>
        /// SelectFatal Property
        /// </summary>
        public bool SelectFatal
        {
            get { return mSelectFatal; }

            set
            {
                if (value != mSelectFatal)
                {
                    mSelectFatal = value;
                    RaisePropertyChanged(PROP_SelectFatal);

                    if (mSelectFatal)
                    {
                        mSelectAll =
                            mShowLevelDebug =
                            mShowLevelInfo = mShowLevelWarn = mShowLevelError = false;
                        mShowLevelFatal = true;
                        RefreshCheckBoxBinding();
                        RefreshView();
                    }

                    CommandClearFilters.CanExecute(null);
                }
            }
        }

        #endregion LogSelectionProperties

        #region Counters

        /// <summary>
        /// ItemsDebugCount Property
        /// </summary>
        public int ItemsDebugCount
        {
            get { return mItemsDebugCount; }
            set
            {
                mItemsDebugCount = value;
                RaisePropertyChanged(PROP_ItemsDebugCount);
            }
        }

        /// <summary>
        /// ItemsInfoCount Property
        /// </summary>
        public int ItemsInfoCount
        {
            get { return mItemsInfoCount; }

            set
            {
                mItemsInfoCount = value;
                RaisePropertyChanged(PROP_ItemsInfoCount);
            }
        }

        /// <summary>
        /// ItemsWarnCount Property
        /// </summary>
        public int ItemsWarnCount
        {
            get { return mItemsWarnCount; }
            set
            {
                mItemsWarnCount = value;
                RaisePropertyChanged(PROP_ItemsWarnCount);
            }
        }

        /// <summary>
        /// ItemsErrorCount Property
        /// </summary>
        public int ItemsErrorCount
        {
            get { return mItemsErrorCount; }
            set
            {
                mItemsErrorCount = value;
                RaisePropertyChanged(PROP_ItemsErrorCount);
            }
        }

        /// <summary>
        /// ItemsFatalCount Property
        /// </summary>
        public int ItemsFatalCount
        {
            get { return mItemsFatalCount; }
            set
            {
                mItemsFatalCount = value;
                RaisePropertyChanged(PROP_ItemsFatalCount);
            }
        }

        /// <summary>
        /// ItemsDebugFilterCount Property
        /// </summary>
        public int ItemsDebugFilterCount
        {
            get { return mItemsDebugFilterCount; }
            set
            {
                mItemsDebugFilterCount = value;
                RaisePropertyChanged(PROP_ItemsDebugFilterCount);
            }
        }

        /// <summary>
        /// ItemsInfoFilterCount Property
        /// </summary>
        public int ItemsInfoFilterCount
        {
            get { return mItemsInfoFilterCount; }
            set
            {
                mItemsInfoFilterCount = value;
                RaisePropertyChanged(PROP_ItemsInfoFilterCount);
            }
        }

        /// <summary>
        /// ItemsWarnFilterCount Property
        /// </summary>
        public int ItemsWarnFilterCount
        {
            get { return mItemsWarnFilterCount; }
            set
            {
                mItemsWarnFilterCount = value;
                RaisePropertyChanged(PROP_ItemsWarnFilterCount);
            }
        }

        /// <summary>
        /// ItemsErrorFilterCount Property
        /// </summary>
        public int ItemsErrorFilterCount
        {
            get { return mItemsErrorFilterCount; }
            set
            {
                mItemsErrorFilterCount = value;
                RaisePropertyChanged(PROP_ItemsErrorFilterCount);
            }
        }

        /// <summary>
        /// ItemsFatalFilterCount Property
        /// </summary>
        public int ItemsFatalFilterCount
        {
            get { return mItemsFatalFilterCount; }
            set
            {
                mItemsFatalFilterCount = value;
                RaisePropertyChanged(PROP_ItemsFatalFilterCount);
            }
        }

        /// <summary>
        /// ItemsFilterCount Property
        /// </summary>
        public int ItemsFilterCount
        {
            get { return mItemsFilterCount; }
            set
            {
                mItemsFilterCount = value;
                RaisePropertyChanged(PROP_ItemsFilterCount);
            }
        }

        #endregion

        #endregion Properties

        #region Methodes

        /// <summary>
        /// Match View Column filter Value (if any) with item property value
        /// and determine if this item should be displayed or not
        /// </summary>
        /// <param name="col"></param>
        /// <param name="logitem"></param>
        /// <returns></returns>
        public static bool MatchTextFilterColumn(ColumnsViewModel col, LogEntry logitem)
        {
            if (col != null)
            {
                foreach (ColumnItem colItem in col.DataGridColumns)
                {
                    // Crashs on filtering LevelIndex field is fixed with the second condition of this if, but it´s not a clean fix...
                    if (string.IsNullOrEmpty(colItem.ColumnFilterValue) == false && !colItem.Field.Equals("LevelIndex"))
                    {
                        object val = GetItemValue(logitem, colItem.Header);

                        if (val != null)
                        {
                            string valToCompare;
                            if (val is DateTime)
                                valToCompare = ((DateTime) val).ToString(GlobalHelper.DisplayDateTimeFormat,
                                                                         CultureInfo.GetCultureInfo
                                                                             (Resources.CultureName));
                            else
                                valToCompare = val.ToString();

                            if (
                                valToCompare.IndexOf(colItem.ColumnFilterValue,
                                                     StringComparison.OrdinalIgnoreCase) < 0)
                                return false;
                        }
                    }
                }
                return true;
            }
            // Why do we return true if the column is null??
            return true;
        }

        /// <summary>
        /// Set the column layout indicated by the <paramref name="columnCollection"/> parameter
        /// </summary>
        /// <param name="columnCollection"></param>
        public void SetColumnsLayout(List<ColumnItem> columnCollection)
        {
            mDataGridColumns.SetColumnsLayout(columnCollection,
                                              ColumnsVmUpdateColumnFilter);
        }

        /// <summary>
        /// Load data of column layouts to re-create column visibility and other layout details
        /// </summary>
        /// <param name="pathFileName"></param>
        public void LoadColumnsLayout(string pathFileName)
        {
            mDataGridColumns.LoadColumnsLayout(pathFileName,
                                               ColumnsVmUpdateColumnFilter);
        }

        /// <summary>
        /// Save data of column layouts to re-create column visibility and other layout details
        /// </summary>
        /// <param name="pathFileName"></param>
        public void SaveColumnsLayout(string pathFileName)
        {
            mDataGridColumns.SaveColumnsLayout(pathFileName);
        }

        internal void UpdateCounters()
        {
            
            ItemsDebugCount = (from it in LogEntryRowViewModels
                                    where it.Entry.LevelIndex.Equals(LevelIndex.DEBUG)
                                    select it).Count();

            ItemsInfoCount = (from it in LogEntryRowViewModels
                                   where it.Entry.LevelIndex.Equals(LevelIndex.INFO)
                                   select it).Count();

            ItemsWarnCount = (from it in LogEntryRowViewModels
                                   where it.Entry.LevelIndex.Equals(LevelIndex.WARN)
                                   select it).Count();

            ItemsErrorCount = (from it in LogEntryRowViewModels
                                    where it.Entry.LevelIndex.Equals(LevelIndex.ERROR)
                                    select it).Count();

            ItemsFatalCount = (from it in LogEntryRowViewModels
                                    where it.Entry.LevelIndex.Equals(LevelIndex.FATAL)
                                    select it).Count();

            RefreshView();
        }

        /// <summary>
        /// Turn filter on or off and refresh the corresponding <see cref="LogItem"/> collection view.
        /// </summary>
        internal void ApplyFilter()
        {
            IsFiltered = !IsFiltered;
            if (IsFiltered)
                UpdateFilteredCounters(LogView);
            else UpdateCounters();
        }

        /// <summary>
        /// Implementation of the Refresh command
        /// </summary>
        /// <param name="callbackOnFinishedparameter"></param>
        internal virtual void CommandRefreshExecute(EvaluateLoadResult callbackOnFinishedparameter)
        {
            if (IsFiltered)
                UpdateFilteredCounters(LogView);
            else
                UpdateCounters();
        }

        /// <summary>
        /// Get the value of a named property and return it,
        /// or null if property name could not be matched.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected static object GetItemValue(object item, string prop)
        {
            object val;
            try
            {
                val = item.GetType().GetProperty(prop).GetValue(item, null);
            }
            catch
            {
                val = null;
            }

            return val;
        }

        /// <summary>
        /// Keep the Actual selected logitems then refresh the view and reset the selected log item
        /// </summary>
        public void RefreshView()
        {
            LogEntryRowViewModel l = SelectedLogItem;
            SelectedLogItem = null;

            if (LogView != null)
            {
                LogView.Refresh();
                RaisePropertyChanged(PROP_LogView);

                // Attempt to restore selected item if there was one before
                // and if it is not part of the filtered set of items
                // (ScrollItemBehaviour may scroll it into view when filter is applied)
                if (l != null)
                {
                    if (OnFilterLogItems(l))
                        SelectedLogItem = l;
                }
            }
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Refresh bindings for all log level filter (Debug, Info, Warn ...) elements
        /// </summary>
        private void RefreshCheckBoxBinding()
        {
            RaisePropertyChanged(PROP_ShowLevelDebug);
            RaisePropertyChanged(PROP_ShowLevelInfo);
            RaisePropertyChanged(PROP_ShowLevelWarn);
            RaisePropertyChanged(PROP_ShowLevelError);
            RaisePropertyChanged(PROP_ShowLevelFatal);
        }

        /// <summary>
        /// Reset the filter selection of all log4net levels (SelectAll, Debug, Info, Warn ...) to false.
        /// </summary>
        private void ResetLevelSelection()
        {
            SelectAll = false;
            SelectDebug = false;
            SelectInfo = false;
            SelectWarn = false;
            SelectError = false;
            SelectFatal = false;
        }

        private void UpdateFilteredCounters(ICollectionView filteredList)
        {
            if (filteredList != null)
            {
                IEnumerable<LogEntryRowViewModel> fltList = filteredList.Cast<LogEntryRowViewModel>();
                if (fltList != null)
                {
                    ItemsFilterCount = fltList.Count();

                    ItemsDebugFilterCount = (from it in fltList
                                             where it.Entry.LevelIndex.Equals(LevelIndex.DEBUG)
                                             select it).Count();

                    ItemsInfoFilterCount = (from it in fltList
                                            where it.Entry.LevelIndex.Equals(LevelIndex.INFO)
                                            select it).Count();

                    ItemsWarnCount = (from it in fltList
                                      where it.Entry.LevelIndex.Equals(LevelIndex.WARN)
                                            select it).Count();

                    ItemsErrorFilterCount = (from it in fltList
                                             where it.Entry.LevelIndex.Equals(LevelIndex.ERROR)
                                             select it).Count();

                    ItemsFatalFilterCount = (from it in fltList
                                             where it.Entry.LevelIndex.Equals(LevelIndex.FATAL)
                                             select it).Count();
                }
            }
            else
            {
                ItemsFilterCount = 0;
                ItemsDebugFilterCount = 0;
                ItemsInfoFilterCount = 0;
                ItemsWarnFilterCount = 0;
                ItemsErrorFilterCount = 0;
                ItemsFatalFilterCount = 0;
            }
            RefreshView();
        }

        /// <summary>
        /// Return true if the supplied item should be filtered 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool LevelCheckFilter(object item)
        {
            var logItemVm = item as LogEntryRowViewModel;

            if (logItemVm != null)
            {
                switch (logItemVm.Entry.LevelIndex)
                {
                    case LevelIndex.DEBUG:
                        return ShowLevelDebug;

                    case LevelIndex.INFO:
                        return ShowLevelInfo;

                    case LevelIndex.WARN:
                        return ShowLevelWarn;

                    case LevelIndex.ERROR:
                        return ShowLevelError;

                    case LevelIndex.FATAL:
                        return ShowLevelFatal;
                }
            }

            return true;
        }

        /// <summary>
        /// Determine if an item in the observable collection is to be filtered or not.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns true if item is not filtered and false otherwise</returns>
        private bool OnFilterLogItems(object item)
        {
            var logitemVm = item as LogEntryRowViewModel;

            if (logitemVm == null)
                return true; // Item is not filtered

            // Evaluate text filters if we are in filter mode, otherwise, display EVERY item!
            if (IsFiltered)
            {
                if (MatchTextFilterColumn(mDataGridColumns, logitemVm.Entry) == false)
                    return false;
                if (_filterViewModel.Evaluate(logitemVm.Entry) == false)
                    return false;
            }

            if (SelectAll == false)
            {
                switch (logitemVm.Entry.LevelIndex)
                {
                    case LevelIndex.DEBUG:
                        return mShowLevelDebug;

                    case LevelIndex.INFO:
                        return mShowLevelInfo;

                    case LevelIndex.WARN:
                        return mShowLevelWarn;

                    case LevelIndex.ERROR:
                        return mShowLevelError;

                    case LevelIndex.FATAL:
                        return mShowLevelFatal;
                }
            }

            return true; // uri.Contains(movie.ID.ToString());
        }

        /// <summary>
        /// Build a CollectionView on top of an observable collection.
        /// This is required to implemented Filter and Group Features
        /// for a DataGrid in an MVVM fashion.
        /// </summary>
        /// <param name="items"></param>
        private void RebuildLogView(ObservableCollection<LogEntryRowViewModel> items)
        {
            if (LogEntryRowViewModels != null)
                foreach (LogEntryRowViewModel item in items)
                    LogEntryRowViewModels.Add(item);
            else
                LogEntryRowViewModels = new ObservableCollection<LogEntryRowViewModel>();
            LogView = (CollectionView) CollectionViewSource.GetDefaultView(LogEntryRowViewModels);
            LogView.Filter = OnFilterLogItems;
            if(IsFiltered)
                UpdateFilteredCounters(LogView);
            else
                UpdateCounters();
        }

        private void RemoveAllItems()
        {
            LogEntryRowViewModels.Clear();
        }

        /// <summary>
        /// Update the contents of the displayed columns if we received a (key) update event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnsVmUpdateColumnFilter(object sender, EventArgs e)
        {
            //// Justification: It might be better to let the user decide when a filter is on or off
            //// since typing can be a slow down experience if the filter is updated on every keyboard press
            ////
            //// if (this.IsFiltered == false)
            ////   this.IsFiltered = true;      // filter toggle will refresh collection view
            ////else
            if (IsFiltered)
            {
                RefreshView(); // otherwise, just refresh if filter is already in place
            }
        }

        /// <summary>
        /// Analyse an exception and return a human read-able string containing messages from the stacktrace
        /// 
        /// Exception Message 1
        ///  +- Exception Message 2
        ///    +- Exception Message 3
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string GetExceptionTreeAsString(Exception exp)
        {
            string messageBoxText = "Unknown error occured.";

            try
            {
                messageBoxText = exp.Message;

                Exception innerEx = exp.InnerException;

                for (int i = 0; innerEx != null; i++, innerEx = innerEx.InnerException)
                {
                    string spaces = string.Empty;

                    for (int j = 0; j < i; j++)
                        spaces += "  ";

                    messageBoxText += "\n" + spaces + "+->" + innerEx.Message;
                }
            }
            catch
            {
            }
            
            return messageBoxText;
        }

        public ICommandAncestor CommandApplyFilter { get; protected set; }

        public ICommandAncestor CommandResetFilter { get; protected set; }

        internal virtual object CommandApplyFilterExecute(object parameter)
        {
            _filterViewModel.AddQuery(_filterViewModel.ActualQuery);
            IsFiltered = true;
            return null;
        }

        internal virtual bool CommandApplyFilterCanExecute(object parameter)
        {
            _filterViewModel.ActualQuery = parameter as string;
            return _filterViewModel.IsQueryValid();
        }


        internal virtual object CommandResetFilterExecute(object parameter)
        {
            UpdateCounters();
            return null;
        }

        internal virtual bool CommandResetFilterCanExecute(object parameter)
        {
            return true;
        }

        #region commandDelete

        internal virtual object CommandDeleteExecute(object parameter)
        {
            return null;
        }

        internal virtual bool CommandDeleteCanExecute(object parameter)
        {
            return false;
        }

        #endregion commandDelete

        #region commandClear

        internal virtual object CommandClearFiltersExecute(object parameter)
        {
            IsFiltered = false; // Reset column text filter
            SelectAll = true; // Reset level classification filter

            ////if (this.DataGridColumns != null)
            ////  this.DataGridColumns.ResetSearchTextBox();

            UpdateCounters();

            return null;
        }

        internal virtual bool CommandClearFilterCanExecute(object parameter)
        {
            if (HasData)
            {
                return (IsFiltered | SelectAll == false);
            }

            return false;
        }

        #endregion commandClear

        #endregion Methodes

        /// <summary>
        /// Reload the LogEntryRowViewModels then update counters and update the selectedlogitem
        /// </summary>
        /// <param name="repositories">Repositories containing the entries</param>
        public void SetEntries(List<RepositoryViewModel> repositories)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                (Action) delegate()
                             {
                                 RemoveAllItems();
                                 foreach (var repo in repositories)
                                 {
                                     if (repo.Active)
                                     {
                                         foreach (LogEntry entry in repo.Repository.LogEntries)
                                         {
                                             LogEntryRowViewModels.Add(new LogEntryRowViewModel(entry));
                                         }
                                     }
                                 }
                                 SelectedLogItem = LogEntryRowViewModels.Any()
                                                       ? LogEntryRowViewModels.Last()
                                                       : null;
                             });
        }
    }
}