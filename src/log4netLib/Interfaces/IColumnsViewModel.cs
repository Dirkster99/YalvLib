namespace log4netLib.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// ViewModel class to organize all items that apply on a column of the log4net (YalvView) column.
    /// These items are the actual column layouts (number and columns and their names) and contents
    /// of the filter search boxes.
    /// </summary>
    public interface IColumnsViewModel
    {
        /// <summary>
        /// Return a list of columns to be displayed in a DataGrid view display
        /// </summary>
        IList<IColumnItem> DataGridColumns{ get; }
    }
}
