namespace YalvLib.ViewModel
{
  using System.Collections.Generic;
  using YalvLib.Common;
  using YalvLib.Domain;

  public class ColumnsVM : BindableObject
  {
    #region fields
    private IList<ColumnItem> mDataGridColumns = null;
    private List<string> mFilterProperties = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public ColumnsVM()
    {
      this.BuidColumns();
    }

    /// <summary>
    /// Clear all data row values displayed in the view.
    /// </summary>
    /// <param name="columnFilterUpdate"></param>
    public ColumnsVM(System.EventHandler columnFilterUpdate)
    {
      this.BuidColumns(columnFilterUpdate);
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Return a list of columns to be displayed in a DataGrid view display
    /// </summary>
    public IList<ColumnItem> DataGridColumns
    {
      get
      {
        return this.mDataGridColumns;
      }
    }

    public List<string> FilterProperties
    {
      get
      {
        return this.mFilterProperties;
      }

      set
      {
        if (this.mFilterProperties != value)
        {
          this.mFilterProperties = value;
          this.OnAfterPropertyChanged("FilterProperties");
        }
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Reset all filter values to empty strings
    /// </summary>
    internal void ResetSearchTextBox()
    {
      if (this.mDataGridColumns != null)
      {
        for (int i = 0; i < this.mDataGridColumns.Count; i++)
        {
          this.mDataGridColumns[i].ColumnFilterValue = string.Empty;
        }
      }
    }

    /// <summary>
    /// Build an intial layout of GridView columns and their text filter textboxes.
    /// </summary>
    /// <param name="columnFilterUpdate"></param>
    private void BuidColumns(System.EventHandler columnFilterUpdate = null)
    {
      try
      {
        this.mDataGridColumns = new List<ColumnItem>()
        {
          new ColumnItem("Id",          32, null, CellAlignment.CENTER, string.Empty) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_IdColumn_Header },
          new ColumnItem("TimeStamp",   32, null, CellAlignment.CENTER, GlobalHelper.DisplayDateTimeFormat) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_TimeStampColumn_Header },
          new ColumnItem("Level",       32, null, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_LevelColumn_Header },
          new ColumnItem("Message",     32, 400) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_MessageColumn_Header },
          new ColumnItem("Logger",      32, null) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_LoggerColumn_Header },
          new ColumnItem("MachineName", 32, null, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_MachineNameColumn_Header },
          new ColumnItem("HostName",    32, null, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_HostNameColumn_Header },
          new ColumnItem("UserName",    32, null, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_UserNameColumn_Header },
          new ColumnItem("App",         32, null) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_AppColumn_Header },
          new ColumnItem("Thread",      32, null, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_ThreadColumn_Header },
          new ColumnItem("Class",       32, null) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_ClassColumn_Header },
          new ColumnItem("Method",      32, null) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_MethodColumn_Header },
          new ColumnItem("Delta",       32, null, CellAlignment.CENTER, null, "Δ"),
          ////new ColumnItem("Path", 32)
        };

        int size = (this.mDataGridColumns == null ? 0 : this.mDataGridColumns.Count);

        this.mFilterProperties = new List<string>(size);
        for (int i = 0; i < size; i++)
        {
          this.mFilterProperties.Add(string.Empty);

          if (columnFilterUpdate != null)
            this.mDataGridColumns[i].UpdateColumnFilter += columnFilterUpdate;
        }
      }
      catch
      {
      }
    }
    #endregion methods
  }
}
