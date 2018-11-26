namespace log4netLib.Controls
{
    using log4netLib.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Implements a WPF control that displays log4net log data entries
    /// in a gridview.
    /// </summary>
    public partial class Log4NetView : Control
    {
        #region fields
        /// <summary>
        /// Dependency property to bind column meta data definitions to the view
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns",
                typeof(IColumnsViewModel), typeof(Log4NetView), new UIPropertyMetadata(null, OnDataGridChanged));

        /// <summary>
        /// Dependency property to bind grid cell default style to the view
        /// </summary>
        public static readonly DependencyProperty CenterCellStyleProperty =
            DependencyProperty.Register("CenterCellStyle",
                typeof(Style), typeof(Log4NetView), new UIPropertyMetadata(null));

        /// <summary>
        /// Dependency property to bind WaterMark textbox style to the view
        /// </summary>
        public static readonly DependencyProperty WaterMarkTextBoxProperty =
            DependencyProperty.Register("WaterMarkTextBox",
                typeof(Style), typeof(Log4NetView), new UIPropertyMetadata(null));

        private readonly GridManager _GridManager;
        private DataGrid _DataGrid;
        private Panel _SearchPanel;
        #endregion fields

        #region constructors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static Log4NetView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Log4NetView), new FrameworkPropertyMetadata(typeof(Log4NetView)));
        }

        /// <summary>
        /// Standard class constructor
        /// </summary>
        public Log4NetView()
        {
            _GridManager = new GridManager();
            Loaded += Log4NetView_Loaded;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Column dependency property
        /// </summary>
        public IColumnsViewModel Columns
        {
            get { return (IColumnsViewModel)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Data grid column cell style dependency property
        /// </summary>
        public Style CenterCellStyle
        {
            get { return (Style)GetValue(CenterCellStyleProperty); }
            set { SetValue(CenterCellStyleProperty, value); }
        }

        /// <summary>
        /// WatermarkTextBox style dependency property
        /// </summary>
        public Style WaterMarkTextBox
        {
            get { return (Style)GetValue(WaterMarkTextBoxProperty); }
            set { SetValue(WaterMarkTextBoxProperty, value); }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Is invoked whenever application code or internal
        /// processes call System.Windows.FrameworkElement.ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _DataGrid = GetTemplateChild("PART_DataGrid") as DataGrid;
            _SearchPanel = GetTemplateChild("PART_SearchPanel") as Panel;
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

            IColumnsViewModel colVM = Columns;

            if (colVM == null)
                return;

            IColumnItem col = colVM.DataGridColumns.FirstOrDefault(f => f.FilterControlName == tb.Name);

            if (col == null)
                return;

            col.SetColumnFilterValue(tb.Text);
        }

        /// <summary>
        /// Re-Build all column headers and corresponding filter textboxes for the datagrid
        /// control whenever the corresponding columns property has changed.
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        private static void OnDataGridChanged(DependencyObject depObj,
                                              DependencyPropertyChangedEventArgs e)
        {
            var control = depObj as Log4NetView;

            if (control == null)
                return;

            if (e.NewValue != null && e.OldValue != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    // Runtime optimization: Execute this only if control was already loaded
                    // (onLoad method initializes otherwise)
                    if (control.IsLoaded)
                        control.RebuildGrid();
                }
                else
                {
                    if (e.NewValue != null && e.OldValue == null)
                    {
                        // Runtime optimization: Execute this only if control was already loaded
                        // (onLoad method initializes otherwise)
                        if (control.IsLoaded)
                            control.RebuildGrid();
                    }
                }
            }
        }

        /// <summary>
        /// Create columns and other items (text filter textbox headers) that bring the DataGrid to live.
        /// </summary>
        private void RebuildGrid()
        {
            IColumnsViewModel colVM = Columns;
            Style centerCellStyle = CenterCellStyle;
            Style watermarkTextbox = WaterMarkTextBox;

            // Check the resource style for this class if any of these items is null
            var partDataGrid = GetTemplateChild("PART_DataGrid") as DataGrid;
            var partSearchPanel = GetTemplateChild("PART_SearchPanel") as Panel;


            if (colVM != null && partDataGrid != null)
            {
                partDataGrid.EnableRowVirtualization = true;
                _GridManager.BuildDataGrid(partDataGrid, colVM,
                                           centerCellStyle, watermarkTextbox,
                                           HandleRefreshOnInputKeyInFilter,
                                           partSearchPanel);

                ////Not necessary since the ViewModel creates the CollectionView and looks after the filtering
                ////this.AssignSource(new Binding("LogEntryRowViewModels") { Source = this, Mode = BindingMode.OneWay });
            }
        }

        private void Log4NetView_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Log4NetView_Loaded;
            RebuildGrid();
            _DataGrid.SelectionChanged += CallUpdateTextMarkers;
            _DataGrid.SelectionChanged += CallUpdateDelta;
        }

        private void CallUpdateTextMarkers(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<ILogEntryRowViewModel> list = _DataGrid.SelectedItems.Cast<ILogEntryRowViewModel>();

            var datacontext = this.DataContext as IYalvViewModel;
            if (datacontext != null)
            {
                if (datacontext.CommandUpdateTextMarkers.CanExecute(list))
                {
                    var ctrl = FocusManager.GetFocusedElement(this) as Control;
                    if (ctrl != null)
                    {
                        if (ctrl.Parent != null && ctrl.Parent.GetType() != typeof(DataGridCell))
                        {
                            System.Console.WriteLine("outside!");
                            return;
                        }
                    }

                    datacontext.CommandUpdateTextMarkers.Execute(list);
                }
            }
        }

        private void CallUpdateDelta(object sender, SelectionChangedEventArgs e)
        {
            IEnumerable<ILogEntryRowViewModel> list = _DataGrid.SelectedItems.Cast<ILogEntryRowViewModel>();

            var datacontext = this.DataContext as IYalvViewModel;
            if (datacontext != null)
            {
                if (datacontext.CommandUpdateDelta.CanExecute(list))
                    datacontext.CommandUpdateDelta.Execute(list);
            }
        }
        #endregion methods
    }
}
