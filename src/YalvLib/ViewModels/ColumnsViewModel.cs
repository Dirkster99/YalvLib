﻿namespace YalvLib.ViewModels
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
        private IList<IColumnItem> _dataGridColumns;
        private List<string> _filterProperties;
        #endregion fields

        #region constructors
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
        #endregion constructors

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
                    _dataGridColumns[i].SetColumnFilterValue(string.Empty);
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
                    _dataGridColumns[i].SetWidth(_dataGridColumns[i].ActualWidth.Width);
            }

            SaveColumnLayout(pathFileName, _dataGridColumns);
        }

        /// <summary>
        /// Set the column layout indicated by the <paramref name="columnCollection"/> parameter
        /// </summary>
        /// <param name="columnFilterUpdate"></param>
        /// <param name="columnCollection"></param>
        internal void SetColumnsLayout(List<IColumnItem> columnCollection,
                                       EventHandler columnFilterUpdate = null)
        {
            try
            {
                if (columnCollection != null)
                    _dataGridColumns = new List<IColumnItem>(columnCollection);
                else
                    _dataGridColumns = new List<IColumnItem>();

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
                _dataGridColumns = new List<IColumnItem>
                {
                    new ColumnItemViewModel("Entry.GuId", 32, 50, CellAlignment.CENTER,null, Resources.MainWindowVM_InitDataGrid_IdMergeColumn_Header),
                    new ColumnItemViewModel("Entry.Id", 32, 25, CellAlignment.CENTER, null, Resources.MainWindowVM_InitDataGrid_IdColumn_Header),
                    new ColumnItemViewModel("Entry.TimeStamp", 32, 100, CellAlignment.CENTER,
                                    GlobalHelper.DisplayDateTimeFormat,
                                    Resources.MainWindowVM_InitDataGrid_TimeStampColumn_Header),
                    new ColumnItemViewModel("Entry.LevelIndex", 32, 50, CellAlignment.CENTER, null, Resources.MainWindowVM_InitDataGrid_LevelColumn_Header),
                    new ColumnItemViewModel("Entry.Message", 32, 400, CellAlignment.DEFAULT, null, Resources.MainWindowVM_InitDataGrid_MessageColumn_Header),
                    new ColumnItemViewModel("Entry.Logger", 32, 100, CellAlignment.DEFAULT, null, Resources.MainWindowVM_InitDataGrid_LoggerColumn_Header),
                    new ColumnItemViewModel("Entry.MachineName", 32, 100, CellAlignment.CENTER, null,Resources.MainWindowVM_InitDataGrid_MachineNameColumn_Header),
                    new ColumnItemViewModel("Entry.HostName", 32, 100, CellAlignment.CENTER, null, Resources.MainWindowVM_InitDataGrid_HostNameColumn_Header),
                    new ColumnItemViewModel("Entry.UserName", 32, 100, CellAlignment.CENTER, null, Resources.MainWindowVM_InitDataGrid_UserNameColumn_Header),
                    new ColumnItemViewModel("Entry.App", 32, 50, CellAlignment.DEFAULT, null,Resources.MainWindowVM_InitDataGrid_AppColumn_Header),
                    new ColumnItemViewModel("Entry.Thread", 32, 50, CellAlignment.CENTER, null,Resources.MainWindowVM_InitDataGrid_ThreadColumn_Header),
                    new ColumnItemViewModel("Entry.Class", 32, 150, CellAlignment.DEFAULT, null,Resources.MainWindowVM_InitDataGrid_ClassColumn_Header),
                    new ColumnItemViewModel("Entry.Method", 32, 150, CellAlignment.DEFAULT, null,Resources.MainWindowVM_InitDataGrid_MethodColumn_Header),
                    new ColumnItemViewModel("Entry.Delta", 32, 50, CellAlignment.CENTER, null, "Δ", false)
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

        private static IList<IColumnItem> LoadColumnLayout(string pathFileName)
        {
            IList<ColumnItemViewModel> loadedClass = null;
            if (File.Exists(pathFileName))
            {
                using (var readFileStream = new FileStream(pathFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    try
                    {
                        var serializerObj = new XmlSerializer(typeof(List<ColumnItemViewModel>));

                        loadedClass = (List<ColumnItemViewModel>)serializerObj.Deserialize(readFileStream);
                    }
                    catch (XmlException e)
                    {
                        Console.WriteLine(string.Format("'{0}' Line: {1} Pos: {2}",
                            e.Message, e.LineNumber, e.LinePosition));

                        Console.WriteLine(e.StackTrace);
                        return null;
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

            if (loadedClass != null)
                return loadedClass.Cast<IColumnItem>().ToList();

            return null;
        }

        private static bool SaveColumnLayout(string settingsFileName,
                                             IList<IColumnItem> iList)
        {
            try
            {
                var vm = new List<ColumnItemViewModel>(iList.Cast<ColumnItemViewModel>());

                var xws = new XmlWriterSettings();
                xws.NewLineOnAttributes = true;
                xws.Indent = true;
                xws.IndentChars = "  ";
                xws.Encoding = Encoding.UTF8;

                using (XmlWriter xw = XmlWriter.Create(settingsFileName, xws))
                {
                    // Create a new XmlSerializer instance with the type of the class
                    var serializerObj = new XmlSerializer(typeof(List<ColumnItemViewModel>));
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