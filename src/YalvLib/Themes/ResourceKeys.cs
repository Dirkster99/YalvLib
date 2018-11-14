namespace YalvLib.Themes
{
    using System.Windows;

    /// <summary>
    /// Class implements static resource keys that should be referenced to configure
    /// colors, styles and other elements that are typically changed between themes.
    /// </summary>
    public static class ResourceKeys
    {
        #region Accent Keys
        /// <summary>
        /// Accent Color Key - This Color key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");

        /// <summary>
        /// Accent Brush Key - This Brush key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");
        #endregion Accent Keys      

        /// <summary>
        /// Gets the color key for the normal control enabled background color.
        /// </summary>
        public static readonly ComponentResourceKey ControlNormalBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundKey");

        /// <summary>
        /// Gets a the applicable foreground Brush key that should be used for coloring text.
        /// </summary>
        public static readonly ComponentResourceKey ControlTextBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlTextBrushKey");

        /// <summary>
        /// Gets the Brush key for the normal control enabled foreground color.
        /// </summary>
        public static readonly ComponentResourceKey ControlNormalForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalForegroundBrushKey");

        /// <summary>
        /// Gets the Brush key of the border color of a control.
        /// </summary>
        public static readonly ComponentResourceKey ControlBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlBorderBrushKey");

        #region DataGridHeader Keys
        /// <summary>
        /// Gets the Brush key of the border color
        /// of the vertical Data Grid Header line between each header item.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderBorderBrushKey");

        /// <summary>
        /// Gets the background color of the datagrid header row.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderBackgroundBrushKey");

        /// <summary>
        /// Gets the foreground color of the datagrid header row.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderForegroundBrushKey");

        /// <summary>
        /// Gets the foreground color of the sort arrow display in the datagrid header row.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderSortArrowForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderSortArrowForegroundBrushKey");

        /// <summary>
        /// Gets the font size of the text displayed in the DataGrid header row.
        /// </summary>
        public static readonly ComponentResourceKey DataGridHeaderFontSize = new ComponentResourceKey(typeof(ResourceKeys), "DataGridHeaderFontSize");
        #endregion DataGridHeader Keys

        /// <summary>
        /// Gets the Brush key of the border color
        /// between each column in the data display area of the DataGrid.
        /// </summary>
        public static readonly ComponentResourceKey VerticalGridLinesBrush = new ComponentResourceKey(typeof(ResourceKeys), "VerticalGridLinesBrush");

        /// <summary>
        /// Gets the Brush key of the border color
        /// between each row in the data display area of the DataGrid.
        /// </summary>
        public static readonly ComponentResourceKey HorizontalGridLinesBrush = new ComponentResourceKey(typeof(ResourceKeys), "HorizontalGridLinesBrush");

        /// <summary>
        /// Gets the foreground brush key of a datagrid cell that is selected and focused.
        /// </summary>
        public static readonly ComponentResourceKey SelectedFocusedCellForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "SelectedFocusedCellForegroundBrushKey");

        /// <summary>
        /// Gets the background brush key of a datagrid cell that is selected and focused.
        /// </summary>
        public static readonly ComponentResourceKey SelectedFocusedCellBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "SelectedFocusedCellBackgroundBrushKey");

        /// <summary>
        /// Gets the foreground brush key of a datagrid cell that is selected but not focused.
        /// </summary>
        public static readonly ComponentResourceKey SelectedCellForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "SelectedCellForegroundBrushKey");

        /// <summary>
        /// Gets the background brush key of a datagrid cell that is selected but not focused.
        /// </summary>
        public static readonly ComponentResourceKey SelectedCellBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "SelectedCellBackgroundBrushKey");

        #region LogLevel BrushKeys
        /// <summary>
        /// Gets the background brush key for the data row that represents a debug level log entry.
        /// </summary>
        public static readonly ComponentResourceKey DebugLevelBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DebugLevelBackgroundBrushKey");

        /// <summary>
        /// Gets the background brush key for the data row that represents a information level log entry.
        /// </summary>
        public static readonly ComponentResourceKey InfoLevelBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "InfoLevelBackgroundBrushKey");

        /// <summary>
        /// Gets the background brush key for the data row that represents a warning level log entry.
        /// </summary>
        public static readonly ComponentResourceKey WarnLevelBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "WarnLevelBackgroundBrushKey");

        /// <summary>
        /// Gets the background brush key for the data row that represents a error level log entry.
        /// </summary>
        public static readonly ComponentResourceKey ErrorLevelBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ErrorLevelBackgroundBrushKey");

        /// <summary>
        /// Gets the background brush key for the data row that represents a fatal level log entry.
        /// </summary>
        public static readonly ComponentResourceKey FatalLevelBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "FatalLevelBackgroundBrushKey");
        #endregion LogLevel BrushKeys
    }
}
