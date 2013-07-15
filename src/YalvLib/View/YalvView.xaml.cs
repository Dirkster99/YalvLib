using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YalvLib.ViewModel;

namespace YalvLib.View
{
    /// <summary>
    /// ValvView is a look-less WPF control that displays log4net log data entries (by default) in a gridview.
    /// </summary>
    public partial class YalvView : Control
    {
        #region fields

        /// <summary>
        /// Dependency property to bind column meta data definitions to the view
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns",
                                        typeof (ColumnsViewModel),
                                        typeof (YalvView),
                                        new UIPropertyMetadata(null, OnDataGridChanged));

        /// <summary>
        /// Dependency property to bind grid cell default style to the view
        /// </summary>
        public static readonly DependencyProperty CenterCellStyleProperty =
            DependencyProperty.Register("CenterCellStyle", typeof (Style), typeof (YalvView),
                                        new UIPropertyMetadata(null));

        /// <summary>
        /// Dependency property to bind WaterMark textbox style to the view
        /// </summary>
        public static readonly DependencyProperty WaterMarkTextBoxProperty =
            DependencyProperty.Register("WaterMarkTextBox", typeof (Style), typeof (YalvView),
                                        new UIPropertyMetadata(null));

        #endregion fields

        #region constructor

        /// <summary>
        /// Static class constructor
        /// </summary>
        static YalvView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (YalvView), new FrameworkPropertyMetadata(typeof (YalvView)));
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public YalvView()
        {
            Loaded += YalvView_Loaded;
        }

        #endregion constructor

        #region properties

        /// <summary>
        /// Class constructor
        /// </summary>
        public ColumnsViewModel Columns
        {
            get { return (ColumnsViewModel) GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Data grid column cell style dependency property
        /// </summary>
        public Style CenterCellStyle
        {
            get { return (Style) GetValue(CenterCellStyleProperty); }
            set { SetValue(CenterCellStyleProperty, value); }
        }


        /// <summary>
        /// WatermarkTextBox style dependency property
        /// </summary>
        public Style WaterMarkTextBox
        {
            get { return (Style) GetValue(WaterMarkTextBoxProperty); }
            set { SetValue(WaterMarkTextBoxProperty, value); }
        }

        #endregion properties

        #region Methods

        private DataGrid DataGrid
        {
            get
            {
                var dataGrid = GetTemplateChild("PART_DataGrid") as DataGrid;
                return dataGrid;
            }
        }

        private YalvViewModel YalvDataContext
        {
            get { return (YalvViewModel) DataContext; }
        }

        /// <summary>
        /// Standard method which is invoked when the style for this control is applied
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Handle the case in which the user has entered a character into any of the filter columns -
        /// update the corresponding filter column string in the viewmodel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleRefreshOnInputKeyInFilter(object sender, KeyEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            ColumnsViewModel colVM = Columns;

            if (colVM == null)
                return;

            ColumnItem col = colVM.DataGridColumns.FirstOrDefault(f => f.FilterControlName == tb.Name);

            if (col == null)
                return;

            col.ColumnFilterValue = tb.Text;
        }

        /// <summary>
        /// Build all column headers and corresponding filter textboxes for the datagrid control
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        private static void OnDataGridChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var yalvControl = depObj as YalvView;

            if (yalvControl == null)
                return;

            if (e.NewValue != null && e.OldValue != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    // Runtime optimization: Execute this only if control was already loaded (onLoad method initializes otherwise)
                    if (yalvControl.IsLoaded)
                        yalvControl.RebuildGrid();
                }
                else
                {
                    if (e.NewValue != null && e.OldValue == null)
                    {
                        // Runtime optimization: Execute this only if control was already loaded (onLoad method initializes otherwise)
                        if (yalvControl.IsLoaded)
                            yalvControl.RebuildGrid();
                    }
                }
            }
        }

        private void YalvView_Loaded(object sender, RoutedEventArgs e)
        {
            RebuildGrid();
            DataGrid.SelectionChanged += CallUpdateTextMarkers;
            DataGrid.SelectionChanged += CallUpdateDelta;
            Loaded -= YalvView_Loaded;
        }

        private void CallUpdateTextMarkers(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<LogEntryRowViewModel> list = DataGrid.SelectedItems.Cast<LogEntryRowViewModel>();
            if (YalvDataContext.CommandUpdateTextMarkers.CanExecute(list))
                YalvDataContext.CommandUpdateTextMarkers.Execute(list);
        }


        private void CallUpdateDelta(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<LogEntryRowViewModel> list = DataGrid.SelectedItems.Cast<LogEntryRowViewModel>();
            if (YalvDataContext.CommandUpdateDelta.CanExecute(list))
                YalvDataContext.CommandUpdateDelta.Execute(list);
        }


        /// <summary>
        /// Create columns and other items (text filter textbox headers) that bring the DataGrid to live.
        /// </summary>
        private void RebuildGrid()
        {
            ColumnsViewModel colVM = Columns;
            Style centerCellStyle = CenterCellStyle;
            Style watermarkTextbox = WaterMarkTextBox;

            // Check the resource style for this class if any of these items is null
            var partDataGrid = GetTemplateChild("PART_DataGrid") as DataGrid;
            var partSearchPanel = GetTemplateChild("PART_SearchPanel") as Panel;


            if (colVM != null && partDataGrid != null)
            {
                partDataGrid.EnableRowVirtualization = true;
                GridManager.BuildDataGrid(partDataGrid, colVM,
                                          centerCellStyle, watermarkTextbox,
                                          HandleRefreshOnInputKeyInFilter,
                                          partSearchPanel);

                ////Not necessary since the ViewModel creates the CollectionView and looks after the filtering
                ////this.AssignSource(new Binding("LogEntryRowViewModels") { Source = this, Mode = BindingMode.OneWay });
            }
        }

        #endregion Methods
    }
}