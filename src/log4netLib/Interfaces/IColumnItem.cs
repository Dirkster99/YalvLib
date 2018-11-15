namespace log4netLib.Interfaces
{
    using log4netLib.Enums;
    using log4netLib.Utils;
    using System;

    /// <summary>
    /// Objects of this class store layout data about a column in a data grid
    /// </summary>
    public interface IColumnItem
    {
        #region events
        /// <summary>
        /// This event is raised when a text character is keyed into a filter textbox
        /// </summary>
        event EventHandler UpdateColumnFilter;
        #endregion events

        #region properties

        /// <summary>
        /// Column header label string
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// Field name of data source
        /// </summary>
        string Field { get; set; }

        /// <summary>
        /// Name of textbox that is used to filter this column.
        /// This field is used to update the column filter when the user
        /// types/edits the textbox content.
        /// </summary>
        string FilterControlName { get; set; }

        /// <summary>
        /// Get/set string that can be used to filter the view by the contents of this column
        /// </summary>
        string ColumnFilterValue { get; set; }

        /// <summary>
        /// Get/set string format for dates (and such data types) in localized context. 
        /// </summary>
        string StringFormat { get; set; }

        /// <summary>
        /// Get/set minimum width of this column.
        /// </summary>
        double MinWidth { get; set; }

        /// <summary>
        /// Get/set actual width of this column.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Get property to bind the actual width of a column to
        /// (this readonly dp requires a dependency property which in turn
        /// requires an inheritance from DependencyObject)
        /// </summary>
        BindSupport ActualWidth { get; }

        /// <summary>
        /// Get/set cell alignment for cells displayed in this column.
        /// </summary>
        CellAlignment Alignment { get; set; }

        /// <summary>
        /// Get/set whether column is visible or not.
        /// </summary>
        bool IsColumnVisible { get; set; }
        #endregion properties

    }
}