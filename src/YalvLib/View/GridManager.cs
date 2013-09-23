namespace YalvLib.View
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Common.Converters;
    using ViewModel;
    using ViewModel.Common;

    public partial class YalvView : Control
    {
        private class GridManager
        {
            private static AdjustValueConverter _mAdjustConverter = null;
            private static BoolToVisibilityConverter _mBoolToVisConverter = null;

            static GridManager()
            {
                _mAdjustConverter = new AdjustValueConverter();
                _mBoolToVisConverter = new BoolToVisibilityConverter();
            }

            /// <summary>
            /// This code builds a list of columns and their corresponding filter textboxes
            /// </summary>
            /// <param name="dataGrid"></param>
            /// <param name="centerCellStyle"></param>
            /// <param name="keyUpEvent"></param>
            /// <param name="txtSearchPanel"></param>
            /// <param name="colVM"></param>
            /// <param name="watermarkTextbox"></param>
            internal static void BuildDataGrid(DataGrid dataGrid, ColumnsViewModel colVM,
                                               Style centerCellStyle, Style watermarkTextbox,
                                               KeyEventHandler keyUpEvent,
                                               Panel txtSearchPanel)
            {
                if (dataGrid == null)
                    return;

                // Remove available columns if there are any
                dataGrid.Columns.Clear();

                if (colVM.DataGridColumns != null)
                {
                    CreateMarkersColumn(dataGrid, txtSearchPanel);
                    foreach (ColumnItem item in colVM.DataGridColumns)
                    {
                        DataGridTextColumn col = new DataGridTextColumn();
                        col.Header = item.Header;

                        // Bind column to visibility property via bool - visibility converter
                        var visiblityBinding = new Binding("IsColumnVisible");
                        visiblityBinding.Source = item;
                        visiblityBinding.Converter = _mBoolToVisConverter;
                        BindingOperations.SetBinding(col, DataGridColumn.VisibilityProperty, visiblityBinding);

                        if (item.Alignment == CellAlignment.CENTER && centerCellStyle != null)
                            col.CellStyle = centerCellStyle;

                        col.MinWidth = item.MinWidth;
                        col.Width = item.Width;

                        Binding bind = new Binding(item.Field) { Mode = BindingMode.OneWay };
                        bind.ConverterCulture =
                            System.Globalization.CultureInfo.GetCultureInfo(Strings.Resources.CultureName);

                        if (!string.IsNullOrWhiteSpace(item.StringFormat))
                            bind.StringFormat = item.StringFormat;

                        col.Binding = bind;

                        // Add column to datagrid
                        dataGrid.Columns.Add(col);

                        BuildTextSearchPanel(keyUpEvent, txtSearchPanel, col, item, watermarkTextbox);
                    }
                }
            }

            private static void BuildTextSearchPanel(KeyEventHandler keyUpEvent,
                                                     Panel txtSearchPanel,
                                                     DataGridTextColumn col,
                                                     ColumnItem columnVm,
                                                     Style watermarkTextbox)
            {
                if (txtSearchPanel != null)
                {
                    var widthBind = new Binding
                    {
                        Path = new PropertyPath("ActualWidth"),
                        Source = col,
                        Mode = BindingMode.OneWay,
                        Converter = _mAdjustConverter,
                        ConverterParameter = "-2"
                    };

                    var visibilityBind = new Binding
                    {
                        Path = new PropertyPath("Visibility"),
                        Source = col,
                        Mode = BindingMode.OneWay,
                    };

                    TextBox txt = new TextBox();

                    if (watermarkTextbox != null)
                        txt.Style = watermarkTextbox;

                    columnVm.FilterControlName = txt.Name = GetTextBoxName(columnVm.Field);
                    txt.ToolTip =
                        string.Format(
                            Strings.Resources.FilteredGridManager_BuildDataGrid_FilterTextBox_Tooltip,
                            columnVm.Header);
                    txt.Tag = txt.ToolTip.ToString().ToLower();
                    txt.Text = string.Empty;
                    txt.AcceptsReturn = false;

                    // Bind width of filter text box to ActualWidth of datagrid column
                    txt.SetBinding(WidthProperty, widthBind);

                    // Bind visibility of text box to visibility of the column
                    txt.SetBinding(VisibilityProperty, visibilityBind);

                    // Bind column to width property to viewmodel to enable its persistence
                    // The save function copies ActualWidth into the Width field and persists it
                    Binding b = new Binding("ActualWidth") { Source = col };
                    BindingOperations.SetBinding(columnVm.ActualWidth, BindSupport.WidthProperty, b);

                    if (keyUpEvent != null)
                        txt.KeyUp += keyUpEvent;

                    RegisterControl(txtSearchPanel, txt.Name, txt);
                    txtSearchPanel.Children.Add(txt);
                }
            }

            private static void CreateMarkersColumn(DataGrid dataGrid, Panel txtSearchPanel)
            {
                var markerCol = new DataGridTemplateColumn();

                markerCol.Header = "Markers";

                DataTemplate txtMarker = dataGrid.FindResource("TextMarkerDataTemplate") as DataTemplate;
                DataTemplate colorMarker = dataGrid.FindResource("ColorMarkerDataTemplate") as DataTemplate;
                DataTemplate bothMarker = dataGrid.FindResource("TextAndColorMarkerDataTemplate") as DataTemplate;
                DataTemplate noMarker = dataGrid.FindResource("NoMarkerDataTemplate") as DataTemplate;

                markerCol.CellTemplateSelector = new MarkerTemplateSelector
                {
                    ColorMarkerTemplate = colorMarker,
                    TextMarkerTemplate = txtMarker,
                    TextAndColorMarkerTemplate = bothMarker,
                    NoMarkerTemplate = noMarker
                };

                dataGrid.Columns.Add(markerCol);
                BuildMarkerSearchPanel(txtSearchPanel, markerCol);
            }


            private static void BuildMarkerSearchPanel(Panel txtSearchPanel,
                                                     DataGridTemplateColumn col)
            {
                if (txtSearchPanel != null)
                {
                    var widthBind = new Binding
                    {
                        Path = new PropertyPath("ActualWidth"),
                        Source = col,
                        Mode = BindingMode.OneWay,
                        Converter = _mAdjustConverter,
                        ConverterParameter = "-2"
                    };

                    var visibilityBind = new Binding
                    {
                        Path = new PropertyPath("Visibility"),
                        Source = col,
                        Mode = BindingMode.OneWay,
                    };

                    var txt = new TextBlock();
                    txt.Text = string.Empty;
                    // Bind width of filter text box to ActualWidth of datagrid column
                    txt.SetBinding(WidthProperty, widthBind);

                    // Bind visibility of text box to visibility of the column
                    txt.SetBinding(VisibilityProperty, visibilityBind);

                    // Bind column to width property to viewmodel to enable its persistence
                    // The save function copies ActualWidth into the Width field and persists it

                    //Binding b = new Binding("ActualWidth") {Source = col};
                    //BindingOperations.SetBinding(TextBlock.ActualWidth, BindSupport.WidthProperty, b);

                    RegisterControl(txtSearchPanel, txt.Name, txt);
                    txtSearchPanel.Children.Add(txt);
                }
            }

            private static void RegisterControl<T>(FrameworkElement element, string controlName, T control)
            {
                if ((T)element.FindName(controlName) != null)
                    element.UnregisterName(controlName);

                element.RegisterName(controlName, control);
            }

            private static string GetTextBoxName(string prop)
            {
                return string.Format("txtFilter{0}", prop).Replace(".", string.Empty);
            }
        }
    }
}