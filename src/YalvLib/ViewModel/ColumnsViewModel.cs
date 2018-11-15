namespace YalvLib.ViewModel
{
    using log4netLib.Enums;
    using log4netLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Xml;
    using System.Xml.Serialization;
    using YalvLib.Common;
    using log4netLib.Strings;
    using System.Linq;

    /// <summary>
    /// ViewModel class to organize all items that apply on a column of the log4net (YalvView) column.
    /// These items are the actual column layouts (number and columns and their names) and contents
    /// of the filter search boxes.
    /// </summary>
    public class ColumnsViewModel : BindableObject, IColumnsViewModel
    {
        #region fields
        private IList<ColumnItem> _dataGridColumns;
        private List<string> _filterProperties;
        #endregion fields

        #region constructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ColumnsViewModel()
        {
            BuidColumns();
        }

        /// <summary>
        /// Clear all data row values displayed in the view.
        /// </summary>
        /// <param name="columnFilterUpdate"></param>
        public ColumnsViewModel(EventHandler columnFilterUpdate)
        {
            BuidColumns(columnFilterUpdate);
        }

        #endregion constructor

        #region properties

        /// <summary>
        /// Return a list of columns to be displayed in a DataGrid view display
        /// </summary>
        public IList<IColumnItem> DataGridColumns
        {
            get { return _dataGridColumns.Cast<IColumnItem>().ToList(); }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Reset all filter values to empty strings
        /// </summary>
        internal void ResetSearchTextBox()
        {
            if (_dataGridColumns != null)
            {
                for (int i = 0; i < _dataGridColumns.Count; i++)
                {
                    _dataGridColumns[i].ColumnFilterValue = string.Empty;
                }
            }
        }

        /// <summary>
        /// Save layout of data columns for re-load at a later time
        /// </summary>
        /// <param name="pathFileName"></param>
        internal void SaveColumnsLayout(string pathFileName)
        {
            if (_dataGridColumns == null)
                return;

            // Copy actual width values into width field for persistence
            for (int i = 0; i < _dataGridColumns.Count; i++)
            {
                if (_dataGridColumns[i].ActualWidth != null)
                    _dataGridColumns[i].Width = _dataGridColumns[i].ActualWidth.Width;
            }

            SaveColumnLayout(pathFileName, _dataGridColumns);
        }

        /// <summary>
        /// Set the column layout indicated by the <paramref name="columnCollection"/> parameter
        /// </summary>
        /// <param name="columnFilterUpdate"></param>
        /// <param name="columnCollection"></param>
        internal void SetColumnsLayout(List<ColumnItem> columnCollection,
                                       EventHandler columnFilterUpdate = null)
        {
            try
            {
                if (columnCollection != null)
                    _dataGridColumns = new List<ColumnItem>(columnCollection);
                else
                    _dataGridColumns = new List<ColumnItem>();

                ResetColumnProperties(columnFilterUpdate);
            }
            catch(Exception e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Load data of column layouts to re-create column visibility and other layout details
        /// </summary>
        /// <param name="pathFileName"></param>
        /// <param name="columnFilterUpdate"></param>
        internal void LoadColumnsLayout(string pathFileName,
                                        EventHandler columnFilterUpdate = null)
        {
            if ((_dataGridColumns = LoadColumnLayout(pathFileName)) == null)
                BuidColumns(columnFilterUpdate);
            else
                ResetColumnProperties(columnFilterUpdate);

            RaisePropertyChanged("DataGridColumns");
        }

        /// <summary>
        /// Build an intial layout of GridView columns and their text filter textboxes.
        /// </summary>
        /// <param name="columnFilterUpdate"></param>
        private void BuidColumns(EventHandler columnFilterUpdate = null)
        {
            try
            {
                _dataGridColumns = new List<ColumnItem>
                {
                    new ColumnItem("Entry.GuId", 32, 50, CellAlignment.CENTER)
                        {Header = Resources.MainWindowVM_InitDataGrid_IdMergeColumn_Header},
                    new ColumnItem("Entry.Id", 32, 25, CellAlignment.CENTER, string.Empty)
                        {Header = Resources.MainWindowVM_InitDataGrid_IdColumn_Header},
                    new ColumnItem("Entry.TimeStamp", 32, 100, CellAlignment.CENTER,
                                    GlobalHelper.DisplayDateTimeFormat)
                        {Header = Resources.MainWindowVM_InitDataGrid_TimeStampColumn_Header},
                    new ColumnItem("Entry.LevelIndex", 32, 50, CellAlignment.CENTER)
                        {Header = Resources.MainWindowVM_InitDataGrid_LevelColumn_Header},
                    new ColumnItem("Entry.Message", 32, 400)
                        {Header = Resources.MainWindowVM_InitDataGrid_MessageColumn_Header},
                    new ColumnItem("Entry.Logger", 32, 100)
                        {Header = Resources.MainWindowVM_InitDataGrid_LoggerColumn_Header},
                    new ColumnItem("Entry.MachineName", 32, 100, CellAlignment.CENTER)
                        {Header = Resources.MainWindowVM_InitDataGrid_MachineNameColumn_Header},
                    new ColumnItem("Entry.HostName", 32, 100, CellAlignment.CENTER)
                        {Header = Resources.MainWindowVM_InitDataGrid_HostNameColumn_Header},
                    new ColumnItem("Entry.UserName", 32, 100, CellAlignment.CENTER)
                        {Header = Resources.MainWindowVM_InitDataGrid_UserNameColumn_Header},
                    new ColumnItem("Entry.App", 32, 50)
                        {Header = Resources.MainWindowVM_InitDataGrid_AppColumn_Header},
                    new ColumnItem("Entry.Thread", 32, 50, CellAlignment.CENTER)
                        {Header = Resources.MainWindowVM_InitDataGrid_ThreadColumn_Header},
                    new ColumnItem("Entry.Class", 32, 150)
                        {Header = Resources.MainWindowVM_InitDataGrid_ClassColumn_Header},
                    new ColumnItem("Entry.Method", 32, 150)
                        {Header = Resources.MainWindowVM_InitDataGrid_MethodColumn_Header},
                    new ColumnItem("Entry.Delta", 32, 50, CellAlignment.CENTER, null, "Δ")
                        {IsColumnVisible = false},
                    ////new ColumnItem("Path", 32)
                };

                ResetColumnProperties(columnFilterUpdate);
            }
            catch(Exception e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Re-compute column properties each time when columns are reset (eg.: Layout is reloaded).
        /// </summary>
        /// <param name="columnFilterUpdate"></param>
        private void ResetColumnProperties(EventHandler columnFilterUpdate = null)
        {
            int size = (_dataGridColumns == null ? 0 : _dataGridColumns.Count);

            _filterProperties = new List<string>(size);
            for (int i = 0; i < size; i++)
            {
                _filterProperties.Add(string.Empty);

                if (columnFilterUpdate != null)
                    _dataGridColumns[i].UpdateColumnFilter += columnFilterUpdate;
            }
        }

        #region Load Save Columns Layout

        private static IList<ColumnItem> LoadColumnLayout(string pathFileName)
        {
            IList<ColumnItem> loadedClass = null;
            if (File.Exists(pathFileName))
            {
                using (var readFileStream = new FileStream(pathFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    try
                    {
                        var serializerObj = new XmlSerializer(typeof(List<ColumnItem>));

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
                var vm = new List<ColumnItem>(iList);

                var xws = new XmlWriterSettings();
                xws.NewLineOnAttributes = true;
                xws.Indent = true;
                xws.IndentChars = "  ";
                xws.Encoding = Encoding.UTF8;

                using (XmlWriter xw = XmlWriter.Create(settingsFileName, xws))
                {
                    // Create a new XmlSerializer instance with the type of the class
                    var serializerObj = new XmlSerializer(typeof(List<ColumnItem>));
                    serializerObj.Serialize(xw, vm);
                    xw.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.StackTrace,
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        #endregion Load Save Columns Layout
        #endregion methods
    }
}