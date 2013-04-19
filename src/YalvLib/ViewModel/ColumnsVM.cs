namespace YalvLib.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Windows;
  using System.Xml;
  using System.Xml.Serialization;
  using YalvLib.Common;

  /// <summary>
  /// ViewModel class to organize all items that apply on a column of the log4net (YalvView) column.
  /// These items are the actual column layouts (number and columns and their names) and contents
  /// of the filter search boxes.
  /// </summary>
  public class ColumnsVM : BindableObject
  {
    #region fields
    private IList<ColumnItem> mDataGridColumns = null;
    private List<string> mFilterProperties = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard constructor
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
    /// Save layout of data columns for re-load at a later time
    /// </summary>
    /// <param name="pathFileName"></param>
    internal void SaveColumnsLayout(string pathFileName)
    {
      if (this.mDataGridColumns == null)
        return;

      // Copy actual width values into width field for persistence
      for (int i = 0; i < this.mDataGridColumns.Count; i++)
      {
        if (this.mDataGridColumns[i].ActualWidth != null)
          this.mDataGridColumns[i].Width = this.mDataGridColumns[i].ActualWidth.Width;
      }

      ColumnsVM.SaveColumnLayout(pathFileName, this.DataGridColumns);
    }

    /// <summary>
    /// Set the column layout indicated by the <paramref name="columnCollection"/> parameter
    /// </summary>
    /// <param name="columnFilterUpdate"></param>
    /// <param name="columnCollection"></param>
    internal void SetColumnsLayout(List<ColumnItem> columnCollection,
                                   System.EventHandler columnFilterUpdate = null)
    {
      try
      {
        if (columnCollection != null)
          this.mDataGridColumns = new List<ColumnItem>(columnCollection);
        else
          this.mDataGridColumns = new List<ColumnItem>();

        this.ResetColumnProperties(columnFilterUpdate);
      }
      catch
      {
      }
    }

    /// <summary>
    /// Load data of column layouts to re-create column visibility and other layout details
    /// </summary>
    /// <param name="pathFileName"></param>
    /// <param name="columnFilterUpdate"></param>
    internal void LoadColumnsLayout(string pathFileName,
                                    System.EventHandler columnFilterUpdate = null)
    {
      if ((this.mDataGridColumns = LoadColumnLayout(pathFileName)) == null)
        this.BuidColumns(columnFilterUpdate);
      else
        this.ResetColumnProperties(columnFilterUpdate);

      this.RaisePropertyChanged("DataGridColumns");
    }

    #region Load Save Columns Layout
    private static IList<ColumnItem> LoadColumnLayout(string pathFileName)
    {
      IList<ColumnItem> loadedClass = null;

      if (System.IO.File.Exists(pathFileName))
      {
        using (FileStream readFileStream = new FileStream(pathFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          try
          {
            XmlSerializer serializerObj = new XmlSerializer(typeof(List<ColumnItem>));

            loadedClass = (List<ColumnItem>)serializerObj.Deserialize(readFileStream);
          }
          catch (Exception e)
          {
            Console.WriteLine(e.ToString());

            return null;
          }
          finally
          {
            readFileStream.Close();
          }
        }
      }

      return loadedClass;
    }

    private static bool SaveColumnLayout(string settingsFileName,
                                         IList<ColumnItem> iList)
    {
      try
      {
        List<ColumnItem> vm = new List<ColumnItem>(iList);

        XmlWriterSettings xws = new XmlWriterSettings();
        xws.NewLineOnAttributes = true;
        xws.Indent = true;
        xws.IndentChars = "  ";
        xws.Encoding = System.Text.Encoding.UTF8;

        using (XmlWriter xw = XmlWriter.Create(settingsFileName, xws))
        {
          // Create a new XmlSerializer instance with the type of the class
          XmlSerializer serializerObj = new XmlSerializer(typeof(List<ColumnItem>));

          serializerObj.Serialize(xw, vm);

          xw.Close();

          return true;
        }
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message, e.StackTrace.ToString(),
                        MessageBoxButton.OK, MessageBoxImage.Error);

        return false;
      }
    }
    #endregion Load Save Columns Layout

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
          new ColumnItem("Id",          32, 25, CellAlignment.CENTER, string.Empty) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_IdColumn_Header },
          new ColumnItem("TimeStamp",   32, 100, CellAlignment.CENTER, GlobalHelper.DisplayDateTimeFormat) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_TimeStampColumn_Header },
          new ColumnItem("Level",       32, 50, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_LevelColumn_Header },
          new ColumnItem("Message",     32, 400) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_MessageColumn_Header },
          new ColumnItem("Logger",      32, 100) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_LoggerColumn_Header },
          new ColumnItem("MachineName", 32, 100, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_MachineNameColumn_Header },
          new ColumnItem("HostName",    32, 100, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_HostNameColumn_Header },
          new ColumnItem("UserName",    32, 100, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_UserNameColumn_Header },
          new ColumnItem("App",         32, 50) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_AppColumn_Header },
          new ColumnItem("Thread",      32, 50, CellAlignment.CENTER) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_ThreadColumn_Header },
          new ColumnItem("Class",       32, 150) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_ClassColumn_Header },
          new ColumnItem("Method",      32, 150) { Header = YalvLib.Strings.Resources.MainWindowVM_InitDataGrid_MethodColumn_Header },
          new ColumnItem("Delta",       32, 50, CellAlignment.CENTER, null, "Δ") { IsColumnVisible = false },
          ////new ColumnItem("Path", 32)
        };

        this.ResetColumnProperties(columnFilterUpdate);
      }
      catch
      {
      }
    }

    /// <summary>
    /// Re-compute column properties each time when columns are reset (eg.: Layout is reloaded).
    /// </summary>
    /// <param name="columnFilterUpdate"></param>
    private void ResetColumnProperties(System.EventHandler columnFilterUpdate = null)
    {
      int size = (this.mDataGridColumns == null ? 0 : this.mDataGridColumns.Count);

      this.mFilterProperties = new List<string>(size);
      for (int i = 0; i < size; i++)
      {
        this.mFilterProperties.Add(string.Empty);

        if (columnFilterUpdate != null)
          this.mDataGridColumns[i].UpdateColumnFilter += columnFilterUpdate;
      }
    }
    #endregion methods
  }
}
