namespace YalvLib.View
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Input;
  using YalvLib.Common.Converters;
  using YalvLib.Domain;
  using YalvLib.ViewModel;

  internal class GridManager
  {
    private static AdjustValueConverter mAdjustConverter = null;

    static GridManager()
    {
      GridManager.mAdjustConverter = new AdjustValueConverter();
    }

    /// <summary>
    /// This code builds a list of columns and their corresponding filter textboxes
    /// </summary>
    /// <param name="dataGrid"></param>
    /// <param name="columns"></param>
    /// <param name="filterPropertyList"></param>
    /// <param name="centerCellStyle"></param>
    /// <param name="keyUpEvent"></param>
    /// <param name="txtSearchPanel"></param>
    internal static void BuildDataGrid(DataGrid dataGrid, ColumnsVM colVM,
                                       Style centerCellStyle, Style watermarkTextbox,
                                       KeyEventHandler keyUpEvent,
                                       Panel txtSearchPanel)
    {
      if (dataGrid == null)
        return;

      if (colVM.FilterProperties == null)
        colVM.FilterProperties = new List<string>();
      else
        colVM.FilterProperties.Clear();

      if (colVM.DataGridColumns != null)
      {
        foreach (ColumnItem item in colVM.DataGridColumns)
        {
          DataGridTextColumn col = new DataGridTextColumn();
          col.Header = item.Header;

          if (item.Alignment == CellAlignment.CENTER && centerCellStyle != null)
            col.CellStyle = centerCellStyle;

          if (item.MinWidth != null)
            col.MinWidth = item.MinWidth.Value;

          if (item.Width != null)
            col.Width = item.Width.Value;

          Binding bind = new Binding(item.Field) { Mode = BindingMode.OneWay };
          bind.ConverterCulture = System.Globalization.CultureInfo.GetCultureInfo(YalvLib.Strings.Resources.CultureName);

          if (!string.IsNullOrWhiteSpace(item.StringFormat))
            bind.StringFormat = item.StringFormat;

          col.Binding = bind;

          // Add column to datagrid
          dataGrid.Columns.Add(col);

          BuildTextSearchPanel(colVM.FilterProperties, keyUpEvent, txtSearchPanel, item, col, item, watermarkTextbox);
        }
      }
    }

    private static void BuildTextSearchPanel(IList<string> filterPropertyList,
                                             KeyEventHandler keyUpEvent,
                                             Panel txtSearchPanel,
                                             ColumnItem item,
                                             DataGridTextColumn col,
                                             ColumnItem columnItem,
                                             Style watermarkTextbox)
    {
      if (txtSearchPanel != null)
      {
        Binding widthBind = new Binding()
        {
          Path = new PropertyPath("ActualWidth"),
          Source = col,
          Mode = BindingMode.OneWay,
          Converter = mAdjustConverter,
          ConverterParameter = "-2"
        };

        TextBox txt = new TextBox();

        if (watermarkTextbox != null)
          txt.Style = watermarkTextbox;

        columnItem.FilterControlName = txt.Name = getTextBoxName(item.Field);
        txt.ToolTip = string.Format(YalvLib.Strings.Resources.FilteredGridManager_BuildDataGrid_FilterTextBox_Tooltip, item.Header);
        txt.Tag = txt.ToolTip.ToString().ToLower();
        txt.Text = string.Empty;
        txt.AcceptsReturn = false;
        txt.SetBinding(TextBox.WidthProperty, widthBind);    // Bind width of filter text box to ActualWidth of datagrid column
        filterPropertyList.Add(item.Field);

        if (keyUpEvent != null)
          txt.KeyUp += keyUpEvent;

        RegisterControl<TextBox>(txtSearchPanel, txt.Name, txt);
        txtSearchPanel.Children.Add(txt);
      }
    }

    private static void RegisterControl<T>(FrameworkElement element, string controlName, T control)
    {
      if ((T)element.FindName(controlName) != null)
        element.UnregisterName(controlName);

      element.RegisterName(controlName, control);
    }

    private static string getTextBoxName(string prop)
    {
      return string.Format("txtFilter{0}", prop).Replace(".", string.Empty);
    }
  }
}
