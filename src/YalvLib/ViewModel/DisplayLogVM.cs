﻿using YalvLib.Model;

namespace YalvLib.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Data;
  using System.Windows.Input;

  using YalvLib.Common;
  using YalvLib.Common.Interfaces;

  /// <summary>
  /// ViewModel class to organize all items relevant to a loaded logfile display
  /// (columns displayed, file name etc).
  /// </summary>
  public class DisplayLogVM : BindableObject
  {
    #region fields
    private const string PROP_FileDir = "FileDir";
    private const string PROP_LogView = "LogView";
    private const string PROP_HasData = "HasData";
    private const string PROP_Items = "Items";
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

    private object lockObject = new object();

    private ColumnsVM mDataGridColumns = null;
    private LogFileVM mLogFile = null;
    private LogFileLoader fileFoader = null;

    private ObservableCollection<LogEntry> mItems;
    private LogEntry mSelectedLogItem;

    private string mGoToLogItemId;

    private bool mIsFiltered = false;
    private bool mShowLevelDebug;
    private bool mShowLevelInfo;
    private bool mShowLevelWarn;
    private bool mShowLevelError;
    private bool mShowLevelFatal;

    private bool mSelectAll;
    private bool mSelectDebug;
    private bool mSelectInfo;
    private bool mSelectWarn;
    private bool mSelectError;
    private bool mSelectFatal;

    private int mItemsDebugCount;
    private int mItemsInfoCount;
    private int mItemsWarnCount;
    private int mItemsErrorCount;
    private int mItemsFatalCount;

    private int mItemsDebugFilterCount;
    private int mItemsInfoFilterCount;
    private int mItemsWarnFilterCount;
    private int mItemsErrorFilterCount;
    private int mItemsFatalFilterCount;
    private int mItemsFilterCount;

    private EvaluateLoadResult loadResultCallback = null;
    #endregion fields

    #region Constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public DisplayLogVM()
    {
      this.CommandClearFilters = new CommandRelay(this.commandClearFiltersExecute, this.commandClearFilterCanExecute);

      this.SelectAll = true;
      this.IsFiltered = false;
      this.Items = new ObservableCollection<LogEntry>();
      this.RebuildLogView(this.Items);

      // Default constructor contains column definitions
      // The callback is invocked when a column filter string item is changed
      // so we know that we should update the viewmodel filter
      this.mDataGridColumns = new ColumnsVM(this.ColumnsVM_UpdateColumnFilter);

      this.mLogFile = new LogFileVM();
    }
    #endregion Constructor

    #region delegate
    /// <summary>
    /// Declare a type of method to be called upon succesful load of log file.
    /// </summary>
    /// <param name="LoadWasSuccessful"></param>
    public delegate void EvaluateLoadResult(bool LoadWasSuccessful);
    #endregion delegate

    #region Properties
    /// <summary>
    /// Clear Command
    /// </summary>
    public ICommandAncestor CommandClearFilters { get; protected set; }

    /// <summary>
    /// Get a list of columns to be displayed in a DataGrid view display
    /// </summary>
    public ColumnsVM DataGridColumns
    {
      get
      {
        return this.mDataGridColumns;
      }
    }

    /// <summary>
    /// Get a list of files and/or directories that can act as a data source.
    /// </summary>
    public LogFileVM LogFile
    {
      get
      {
        return this.mLogFile;
      }

      set
      {
        if (this.mLogFile != value)
        {
          this.mLogFile = value;
          this.RaisePropertyChanged(PROP_FileDir);
        }
      }
    }

    /// <summary>
    /// This property represents the datagrid viewmodel part and enables sorting and filtering
    /// being implemented in the viewmodel class.
    /// </summary>
    public CollectionView LogView { get; private set; }

    #region LogProperties
    /// <summary>
    /// SelectedLogItem Property
    /// </summary>
    public LogEntry SelectedLogItem
    {
      get
      {
        return this.mSelectedLogItem;
      }

      set
      {
        this.mSelectedLogItem = value;
        this.RaisePropertyChanged(PROP_SelectedLogItem);

        this.mGoToLogItemId = this.mSelectedLogItem != null ? this.mSelectedLogItem.Id.ToString() : string.Empty;
        this.RaisePropertyChanged(DisplayLogVM.PROP_GoToLogItemId);
      }
    }

    /// <summary>
    /// GoToLogItemId Property
    /// </summary>
    public string GoToLogItemId
    {
      get
      {
        return this.mGoToLogItemId;
      }

      set
      {
        this.mGoToLogItemId = value;

        int idGoTo = 0;
        int.TryParse(value, out idGoTo);
        UInt32 currentId = this.SelectedLogItem != null ? this.SelectedLogItem.Id : 0;

        if (idGoTo > 0 && idGoTo != currentId)
        {
          var selectItem = (from it in this.Items
                            where it.Id == idGoTo
                            select it).FirstOrDefault<LogEntry>();

          if (selectItem != null)
            this.SelectedLogItem = selectItem;
        }
        else
          this.mGoToLogItemId = currentId != 0 ? currentId.ToString() : string.Empty;

        this.RaisePropertyChanged(PROP_GoToLogItemId);
      }
    }
    #endregion LogProperties

    #region LogFilterProperties
    /// <summary>
    /// Get/set property to determine whether CollectionView is
    /// is filtered by a column's content or not.
    /// </summary>
    public bool IsFiltered
    {
      get
      {
        return this.mIsFiltered;
      }

      set
      {
        if (this.mIsFiltered != value)
        {
          this.mIsFiltered = value;
          this.RaisePropertyChanged("IsFiltered");
          this.RefreshView();

          this.CommandClearFilters.OnCanExecuteChanged();
        }
      }
    }

    /// <summary>
    /// ShowLevelDebug Property
    /// </summary>
    public bool ShowLevelDebug
    {
      get
      {
        return this.mShowLevelDebug;
      }

      set
      {
        if (value != this.mShowLevelDebug)
        {
          this.mShowLevelDebug = value;
          this.RaisePropertyChanged(PROP_ShowLevelDebug);
          this.resetLevelSelection();
          this.RefreshView();
        }
      }
    }

    /// <summary>
    /// ShowLevelInfo Property
    /// </summary>
    public bool ShowLevelInfo
    {
      get
      {
        return this.mShowLevelInfo;
      }

      set
      {
        if (value != this.mShowLevelInfo)
        {
          this.mShowLevelInfo = value;
          this.RaisePropertyChanged(PROP_ShowLevelInfo);
          this.resetLevelSelection();
          this.RefreshView();
        }
      }
    }

    /// <summary>
    /// ShowLevelWarn Property
    /// </summary>
    public bool ShowLevelWarn
    {
      get
      {
        return this.mShowLevelWarn;
      }

      set
      {
        if (value != this.mShowLevelWarn)
        {
          this.mShowLevelWarn = value;
          this.RaisePropertyChanged(PROP_ShowLevelWarn);
          this.resetLevelSelection();
          this.RefreshView();
        }
      }
    }

    /// <summary>
    /// ShowLevelError Property
    /// </summary>
    public bool ShowLevelError
    {
      get
      {
        return this.mShowLevelError;
      }

      set
      {
        if (value != this.mShowLevelError)
        {
          this.mShowLevelError = value;
          this.RaisePropertyChanged(PROP_ShowLevelError);
          this.resetLevelSelection();
          this.RefreshView();
        }
      }
    }

    /// <summary>
    /// ShowLevelFatal Property
    /// </summary>
    public bool ShowLevelFatal
    {
      get
      {
        return this.mShowLevelFatal;
      }

      set
      {
        if (value != this.mShowLevelFatal)
        {
          this.mShowLevelFatal = value;
          this.RaisePropertyChanged(PROP_ShowLevelFatal);
          this.resetLevelSelection();
          this.RefreshView();
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
      get
      {
        return this.mSelectAll;
      }

      set
      {
        if (value != this.mSelectAll)
        {
          this.mSelectAll = value;
          this.RaisePropertyChanged(PROP_SelectAll);

          if (this.mSelectAll)
          {
            this.mShowLevelDebug = this.mShowLevelInfo = this.mShowLevelWarn = this.mShowLevelError = this.mShowLevelFatal = true;
            this.refreshCheckBoxBinding();
            this.RefreshView();
          }

          this.CommandClearFilters.OnCanExecuteChanged();
        }
      }
    }

    /// <summary>
    /// SelectDebug Property
    /// </summary>
    public bool SelectDebug
    {
      get
      {
        return this.mSelectDebug;
      }

      set
      {
        if (value != this.mSelectDebug)
        {
          this.mSelectDebug = value;
          this.RaisePropertyChanged(PROP_SelectDebug);

          if (this.mSelectDebug)
          {
            this.mSelectAll = this.mShowLevelInfo = this.mShowLevelWarn = this.mShowLevelError = this.mShowLevelFatal = false;
            this.mShowLevelDebug = true;
            this.refreshCheckBoxBinding();
            this.RefreshView();

            this.CommandClearFilters.OnCanExecuteChanged();
          }
        }
      }
    }

    /// <summary>
    /// SelectInfo Property
    /// </summary>
    public bool SelectInfo
    {
      get
      {
        return this.mSelectInfo;
      }

      set
      {
        if (value != this.mSelectInfo)
        {
          this.mSelectInfo = value;
          this.RaisePropertyChanged(PROP_SelectInfo);

          if (this.mSelectInfo)
          {
            this.mSelectAll = this.mShowLevelDebug = this.mShowLevelWarn = this.mShowLevelError = this.mShowLevelFatal = false;
            this.mShowLevelInfo = true;
            this.refreshCheckBoxBinding();
            this.RefreshView();

            this.CommandClearFilters.OnCanExecuteChanged();
          }
        }
      }
    }

    /// <summary>
    /// SelectWarn Property
    /// </summary>
    public bool SelectWarn
    {
      get
      {
        return this.mSelectWarn;
      }

      set
      {
        if (value != this.mSelectWarn)
        {
          this.mSelectWarn = value;
          this.RaisePropertyChanged(PROP_SelectWarn);

          if (this.mSelectWarn)
          {
            this.mSelectAll = this.mShowLevelDebug = this.mShowLevelInfo = this.mShowLevelError = this.mShowLevelFatal = false;
            this.mShowLevelWarn = true;
            this.refreshCheckBoxBinding();
          }

          this.RefreshView();

          this.CommandClearFilters.OnCanExecuteChanged();
        }
      }
    }

    /// <summary>
    /// SelectError Property
    /// </summary>
    public bool SelectError
    {
      get
      {
        return this.mSelectError;
      }

      set
      {
        if (value != this.mSelectError)
        {
          this.mSelectError = value;
          this.RaisePropertyChanged(PROP_SelectError);

          if (this.mSelectError)
          {
            this.mSelectAll = this.mShowLevelDebug = this.mShowLevelInfo = this.mShowLevelWarn = this.mShowLevelFatal = false;
            this.mShowLevelError = true;
            this.refreshCheckBoxBinding();
            this.RefreshView();
          }

          this.CommandClearFilters.OnCanExecuteChanged();
        }
      }
    }

    /// <summary>
    /// SelectFatal Property
    /// </summary>
    public bool SelectFatal
    {
      get
      {
        return this.mSelectFatal;
      }

      set
      {
        if (value != this.mSelectFatal)
        {
          this.mSelectFatal = value;
          this.RaisePropertyChanged(PROP_SelectFatal);

          if (this.mSelectFatal)
          {
            this.mSelectAll = this.mShowLevelDebug = this.mShowLevelInfo = this.mShowLevelWarn = this.mShowLevelError = false;
            this.mShowLevelFatal = true;
            this.refreshCheckBoxBinding();
            this.RefreshView();
          }

          this.CommandClearFilters.OnCanExecuteChanged();
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
      get { return this.mItemsDebugCount; }
      set
      {
        this.mItemsDebugCount = value;
        RaisePropertyChanged(PROP_ItemsDebugCount);
      }
    }

    /// <summary>
    /// ItemsInfoCount Property
    /// </summary>
    public int ItemsInfoCount
    {
      get { return this.mItemsInfoCount; }
      set
      {
        this.mItemsInfoCount = value;
        RaisePropertyChanged(PROP_ItemsInfoCount);
      }
    }

    /// <summary>
    /// ItemsWarnCount Property
    /// </summary>
    public int ItemsWarnCount
    {
      get { return this.mItemsWarnCount; }
      set
      {
        this.mItemsWarnCount = value;
        RaisePropertyChanged(PROP_ItemsWarnCount);
      }
    }

    /// <summary>
    /// ItemsErrorCount Property
    /// </summary>
    public int ItemsErrorCount
    {
      get { return this.mItemsErrorCount; }
      set
      {
        this.mItemsErrorCount = value;
        RaisePropertyChanged(PROP_ItemsErrorCount);
      }
    }

    /// <summary>
    /// ItemsFatalCount Property
    /// </summary>
    public int ItemsFatalCount
    {
      get { return this.mItemsFatalCount; }
      set
      {
        this.mItemsFatalCount = value;
        RaisePropertyChanged(PROP_ItemsFatalCount);
      }
    }

    /// <summary>
    /// ItemsDebugFilterCount Property
    /// </summary>
    public int ItemsDebugFilterCount
    {
      get { return this.mItemsDebugFilterCount; }
      set
      {
        this.mItemsDebugFilterCount = value;
        RaisePropertyChanged(PROP_ItemsDebugFilterCount);
      }
    }

    /// <summary>
    /// ItemsInfoFilterCount Property
    /// </summary>
    public int ItemsInfoFilterCount
    {
      get { return this.mItemsInfoFilterCount; }
      set
      {
        this.mItemsInfoFilterCount = value;
        RaisePropertyChanged(PROP_ItemsInfoFilterCount);
      }
    }

    /// <summary>
    /// ItemsWarnFilterCount Property
    /// </summary>
    public int ItemsWarnFilterCount
    {
      get { return this.mItemsWarnFilterCount; }
      set
      {
        this.mItemsWarnFilterCount = value;
        RaisePropertyChanged(PROP_ItemsWarnFilterCount);
      }
    }

    /// <summary>
    /// ItemsErrorFilterCount Property
    /// </summary>
    public int ItemsErrorFilterCount
    {
      get { return this.mItemsErrorFilterCount; }
      set
      {
        this.mItemsErrorFilterCount = value;
        RaisePropertyChanged(PROP_ItemsErrorFilterCount);
      }
    }

    /// <summary>
    /// ItemsFatalFilterCount Property
    /// </summary>
    public int ItemsFatalFilterCount
    {
      get { return this.mItemsFatalFilterCount; }
      set
      {
        this.mItemsFatalFilterCount = value;
        RaisePropertyChanged(PROP_ItemsFatalFilterCount);
      }
    }

    /// <summary>
    /// ItemsFilterCount Property
    /// </summary>
    public int ItemsFilterCount
    {
      get { return this.mItemsFilterCount; }
      set
      {
        this.mItemsFilterCount = value;
        RaisePropertyChanged(PROP_ItemsFilterCount);
      }
    }
    #endregion

    /// <summary>
    /// Get fewer there are data items in the collection or not
    /// (there may be no items to display if filter is applied but thats a different issue)
    /// </summary>
    internal bool HasData
    {
      get
      {
        return (this.mItems == null ? false : (this.mItems.Count == 0 ? false : true));
      }
    }

    /// <summary>
    /// LogItems property which is the main list of logitems
    /// (this property is bound to a view via CollectionView property)
    /// </summary>
    protected ObservableCollection<LogEntry> Items
    {
      get
      {
        return this.mItems;
      }

      set
      {
        this.mItems = value;
      }
    }
    #endregion Properties

    #region Methodes
    /// <summary>
    /// Match View Column filter Value (if any) with item property value
    /// and determine if this item should be displayed or not
    /// </summary>
    /// <param name="col"></param>
    /// <param name="logitem"></param>
    /// <returns></returns>
    public static bool MatchTextFilterColumn(ColumnsVM col, LogEntry logitem)
    {
      if (col != null)
      {
        foreach (ColumnItem colItem in col.DataGridColumns)
        {
          if (string.IsNullOrEmpty(colItem.ColumnFilterValue) == false)
          {
            object val = getItemValue(logitem, colItem.Header);

            if (val != null)
            {
              string valToCompare = string.Empty;
              if (val is DateTime)
                valToCompare = ((DateTime)val).ToString(GlobalHelper.DisplayDateTimeFormat, System.Globalization.CultureInfo.GetCultureInfo(YalvLib.Strings.Resources.CultureName));
              else
                valToCompare = val.ToString();

              if (valToCompare.ToString().IndexOf(colItem.ColumnFilterValue, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
              else
                return false;
            }
          }
        }
      }

      return true;
    }

    /// <summary>
    /// Set the column layout indicated by the <paramref name="columnCollection"/> parameter
    /// </summary>
    /// <param name="columnCollection"></param>
    public void SetColumnsLayout(List<ColumnItem> columnCollection)
    {
      this.mDataGridColumns.SetColumnsLayout(columnCollection,
                                             this.ColumnsVM_UpdateColumnFilter);
    }

    /// <summary>
    /// Load data of column layouts to re-create column visibility and other layout details
    /// </summary>
    /// <param name="pathFileName"></param>
    public void LoadColumnsLayout(string pathFileName)
    {
      this.mDataGridColumns.LoadColumnsLayout(pathFileName,
                                              this.ColumnsVM_UpdateColumnFilter);
    }

    /// <summary>
    /// Save data of column layouts to re-create column visibility and other layout details
    /// </summary>
    /// <param name="pathFileName"></param>
    public void SaveColumnsLayout(string pathFileName)
    {
      this.mDataGridColumns.SaveColumnsLayout(pathFileName);
    }

    internal void updateCounters()
    {
      this.ItemsDebugCount = (from it in this.Items
                              where it.LevelIndex.Equals(LevelIndex.DEBUG)
                              select it).Count();

      this.ItemsInfoCount = (from it in this.Items
                             where it.LevelIndex.Equals(LevelIndex.DEBUG)
                             select it).Count();

      this.ItemsWarnCount = (from it in this.Items
                             where it.LevelIndex.Equals(LevelIndex.WARN)
                             select it).Count();

      this.ItemsErrorCount = (from it in this.Items
                              where it.LevelIndex.Equals(LevelIndex.ERROR)
                              select it).Count();

      this.ItemsFatalCount = (from it in this.Items
                              where it.LevelIndex.Equals(LevelIndex.FATAL)
                              select it).Count();

      ////this.RefreshView();
    }

    /// <summary>
    /// Turn filter on or off and refresh the corresponding <see cref="LogItem"/> collection view.
    /// </summary>
    internal void ApplyFilter()
    {
      this.IsFiltered = !this.IsFiltered;
    }

    /// <summary>
    /// Implementation of the Refresh command
    /// </summary>
    /// <param name="callbackOnFinishedparameter"></param>
    internal virtual void commandRefreshExecute(EvaluateLoadResult callbackOnFinishedparameter)
    {
      if (this.LogFile.IsFileLoaded == true)
      {
        this.LoadFile(this.LogFile.FilePath, callbackOnFinishedparameter);
      }
    }

    #region commandDelete
    internal virtual object commandDeleteExecute(object parameter)
    {
      this.mLogFile.commandDeleteExecute();

      return null;
    }

    internal virtual bool commandDeleteCanExecute(object parameter)
    {
      return this.mLogFile.commandDeleteCanExecute();
    }
    #endregion commandDelete

    #region commandClear
    internal virtual object commandClearFiltersExecute(object parameter)
    {
      this.IsFiltered = false;   // Reset column text filter
      this.SelectAll = true;    // Reset level classification filter

      ////if (this.DataGridColumns != null)
      ////  this.DataGridColumns.ResetSearchTextBox();

      this.RefreshView();

      return null;
    }

    internal virtual bool commandClearFilterCanExecute(object parameter)
    {
      if (this.HasData == true)
      {
        return (this.IsFiltered == true | this.SelectAll == false);
      }

      return false;
    }
    #endregion commandClear

    /// <summary>
    /// This function is calles to initiate a load file process.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callbackOnFinished"></param>
    /// <returns></returns>
    internal bool LoadFile(string path, EvaluateLoadResult callbackOnFinished)
    {
      lock (this.lockObject)
      {
        if (this.loadResultCallback != null)
          return false;

        // Remember callback delegate for update functions to be used later
        this.loadResultCallback = callbackOnFinished;

        if (this.fileFoader == null)
        {
          this.fileFoader = new LogFileLoader();
          this.fileFoader.loadResultEvent += new EventHandler<LogFileLoader.ResultEvent>(this.fileFoader_ResultEvent);

          this.LogFile.IsLoading = true;
          this.LogFile.FilePath = path;

          this.fileFoader.LoadFile(path, true);

          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Get the value of a named property and return it,
    /// or null if property name could not be matched.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="prop"></param>
    /// <returns></returns>
    protected static object getItemValue(object item, string prop)
    {
      object val = null;
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

    private void RefreshView()
    {
        LogEntry l = this.SelectedLogItem;
      this.SelectedLogItem = null;

      if (this.LogView != null)
      {
        this.LogView.Refresh();
        this.RaisePropertyChanged(DisplayLogVM.PROP_LogView);

        // Attempt to restore selected item if there was one before
        // and if it is not part of the filtered set of items
        // (ScrollItemBehaviour may scroll it into view when filter is applied)
        if (l != null)
        {
          if (this.OnFilterLogItems(l) == true)
            this.SelectedLogItem = l;
        }
      }

      CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Refresh bindings for all log level filter (Debug, Info, Warn ...) elements
    /// </summary>
    private void refreshCheckBoxBinding()
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
    private void resetLevelSelection()
    {
      this.SelectAll = false;
      this.SelectDebug = false;
      this.SelectInfo = false;
      this.SelectWarn = false;
      this.SelectError = false;
      this.SelectFatal = false;
    }

    private void updateFilteredCounters(ICollectionView filteredList)
    {
      if (filteredList != null)
      {
          IEnumerable<LogEntry> fltList = filteredList.Cast<LogEntry>();
        if (fltList != null)
        {
          this.ItemsFilterCount = fltList.Count();

          this.ItemsDebugFilterCount = (from it in fltList
                                        where it.LevelIndex.Equals(LevelIndex.DEBUG)
                                        select it).Count();

          this.ItemsInfoFilterCount = (from it in fltList
                                       where it.LevelIndex.Equals(LevelIndex.INFO)
                                       select it).Count();

          this.ItemsWarnFilterCount = (from it in fltList
                                       where it.LevelIndex.Equals(LevelIndex.WARN)
                                       select it).Count();

          this.ItemsErrorFilterCount = (from it in fltList
                                        where it.LevelIndex.Equals(LevelIndex.ERROR)
                                        select it).Count();

          this.ItemsFatalFilterCount = (from it in fltList
                                        where it.LevelIndex.Equals(LevelIndex.FATAL)
                                        select it).Count();
        }
      }
      else
      {
        this.ItemsFilterCount = 0;
        this.ItemsDebugFilterCount = 0;
        this.ItemsInfoFilterCount = 0;
        this.ItemsWarnFilterCount = 0;
        this.ItemsErrorFilterCount = 0;
        this.ItemsFatalFilterCount = 0;
      }
    }

    /// <summary>
    /// Return true if the supplied item should be filtered 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool levelCheckFilter(object item)
    {
        LogEntry logItem = item as LogEntry;

      if (logItem != null)
      {
        switch (logItem.LevelIndex)
        {
          case LevelIndex.DEBUG:
            return this.ShowLevelDebug;

          case LevelIndex.INFO:
            return this.ShowLevelInfo;

          case LevelIndex.WARN:
            return this.ShowLevelWarn;

          case LevelIndex.ERROR:
            return this.ShowLevelError;

          case LevelIndex.FATAL:
            return this.ShowLevelFatal;
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
        LogEntry logitem = item as LogEntry;

      if (logitem == null)
        return true;        // Item is not filtered

      // Evaluate text filters if we are in filter mode, otherwise, display EVERY item!
      if (this.IsFiltered == true)
      {
        if (MatchTextFilterColumn(this.mDataGridColumns, logitem) == false)
          return false;
      }

      if (this.SelectAll == false)
      {
        switch (logitem.LevelIndex)
        {
          case LevelIndex.DEBUG:
            if (this.mShowLevelDebug == true)
              return true;
            else
              return false;

          case LevelIndex.INFO:
            if (this.mShowLevelInfo == true)
              return true;
            else
              return false;

          case LevelIndex.WARN:
            if (this.mShowLevelWarn == true)
              return true;
            else
              return false;

          case LevelIndex.ERROR:
            if (this.mShowLevelError == true)
              return true;
            else
              return false;

          case LevelIndex.FATAL:
            if (this.mShowLevelFatal == true)
              return true;
            else
              return false;
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
    private void RebuildLogView(ObservableCollection<LogEntry> items)
    {
        this.Items = new ObservableCollection<LogEntry>(items);
      this.LogView = (CollectionView)CollectionViewSource.GetDefaultView(items);
      this.LogView.Filter = this.OnFilterLogItems;
      this.RefreshView();
    }
    
    private void RemoveAllItems()
    {
      this.Items.Clear();
    }

    /// <summary>
    /// Update the contents of the displayed columns if we received a (key) update event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ColumnsVM_UpdateColumnFilter(object sender, System.EventArgs e)
    {
      //// Justification: It might be better to let the user decide when a filter is on or off
      //// since typing can be a slow down experience if the filter is updated on every keyboard press
      ////
      //// if (this.IsFiltered == false)
      ////   this.IsFiltered = true;      // filter toggle will refresh collection view
      ////else
      if (this.IsFiltered == true)
      {
        this.RefreshView();               // otherwise, just refresh if filter is already in place
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

    /// <summary>
    /// This function is called as soon as the load log file parser finishes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void fileFoader_ResultEvent(object sender, LogFileLoader.ResultEvent e)
    {
      lock (this.lockObject)
      {
        bool result = (!e.Cancel) || (!e.Error);

        try
        {
          try
          {
            this.fileFoader.loadResultEvent -= this.fileFoader_ResultEvent;
          }
          catch
          {
          }

          if (e.Cancel == true || e.Error == true)
          {
            string errorMess = e.Message;

            // Extend error message if there is more data available (exception message is usually more relevant than generic message)
            if (e.InnerException != null)
              errorMess = string.Format("{0}\n\n({1})", GetExceptionTreeAsString(e.InnerException), errorMess);

            MessageBox.Show(string.Format(YalvLib.Strings.Resources.MainWindowVM_bkLoaderCompleted_UnreadableFile_Text, errorMess),
                                          YalvLib.Strings.Resources.MainWindowVM_bkLoaderCompleted_UnreadableFile_Title,
                                          MessageBoxButton.OK, MessageBoxImage.Exclamation);

            this.LogFile.IsLoading = false;

            return;
          }

          if (!e.Cancel && e.ResultObjects != null)
          {
            object o;
            e.ResultObjects.TryGetValue(LogFileLoader.KeyLogItems, out o);

            IList<LogEntry> list = o as IList<LogEntry>;

            // (Re-)load file and display content in view
            if (list != null)
                this.RebuildLogView(new ObservableCollection<LogEntry>(list));
            else
              this.RemoveAllItems();

            // Always update views
            this.SelectedLogItem = null;
            this.updateCounters();
            this.RefreshView();

            if (this.Items.Count > 0)         // select the last item in the list to scroll down the view
            {
              var lastItem = (from it in this.Items
                              where this.levelCheckFilter(it)
                              select it).LastOrDefault<LogEntry>();

              // Select the last item to scroll viewer down to last entry
              this.SelectedLogItem = lastItem != null ? lastItem : this.Items[this.Items.Count - 1];
            }
          }

          this.LogFile.IsFileLoaded = true;
          this.LogFile.IsLoading = false;
        }
        finally
        {
          this.fileFoader = null;

          if (this.loadResultCallback != null)
          {
            this.loadResultCallback(result);
            this.loadResultCallback = null;
          }
        }
      }
    }
    #endregion Methodes
  }
}