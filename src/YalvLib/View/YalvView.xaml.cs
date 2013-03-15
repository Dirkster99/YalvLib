namespace YalvLib.View
{
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Input;
  using YalvLib.Domain;
  using YalvLib.ViewModel;

  public class YalvView : Control
  {
    #region fields
    public static readonly DependencyProperty ColumnsProperty =
    DependencyProperty.Register("Columns",
                                typeof(ColumnsVM),
                                typeof(YalvView),
                                new UIPropertyMetadata(null, YalvView.OnDataGridChanged));

    public static readonly DependencyProperty CenterCellStyleProperty =
        DependencyProperty.Register("CenterCellStyle", typeof(Style), typeof(YalvView), new UIPropertyMetadata(null));

    // Using a DependencyProperty as the backing store for WaterMarkTextBox.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty WaterMarkTextBoxProperty =
        DependencyProperty.Register("WaterMarkTextBox", typeof(Style), typeof(YalvView), new UIPropertyMetadata(null));
    #endregion fields

    #region constructor
    static YalvView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(YalvView), new FrameworkPropertyMetadata(typeof(YalvView)));
    }

    public YalvView()
    {
      this.Loaded += new RoutedEventHandler(this.YalvView_Loaded);
    }
    #endregion constructor

    #region properties
    public ColumnsVM Columns
    {
      get { return (ColumnsVM)GetValue(ColumnsProperty); }
      set { SetValue(ColumnsProperty, value); }
    }

    public Style CenterCellStyle
    {
      get { return (Style)GetValue(CenterCellStyleProperty); }
      set { SetValue(CenterCellStyleProperty, value); }
    }

    public Style WaterMarkTextBox
    {
      get { return (Style)GetValue(WaterMarkTextBoxProperty); }
      set { SetValue(WaterMarkTextBoxProperty, value); }
    }
    #endregion properties

    #region Methods
    /// <summary>
    /// Standard method which is invoked when the style for this control is applied
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
    }

    /// <summary>
    /// Handle the case in which the user has entered a character into any of the filter columns
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void HandleRefreshOnInputKeyInFilter(object sender, KeyEventArgs e)
    {
      TextBox tb = sender as TextBox;

      if (tb == null)
        return;

      ColumnsVM colVM = this.Columns;

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
      YalvView yalvControl = depObj as YalvView;

      if (yalvControl == null)
        return;

      // Runtime optimization: Execute this only if control was already loaded (onLoad method initializes otherwise)
      if (yalvControl.IsLoaded == true)
        yalvControl.RebuildGrid();
    }

    private void YalvView_Loaded(object sender, RoutedEventArgs e)
    {
      this.RebuildGrid();
    }

    /// <summary>
    /// Create columns and other items (text filter textbox headers) that bring the DataGrid to live.
    /// </summary>
    private void RebuildGrid()
    {
      ColumnsVM colVM = this.Columns;
      Style centerCellStyle = this.CenterCellStyle;
      Style watermarkTextbox = this.WaterMarkTextBox;

      // Check the resource style for this class if any of these items is null
      DataGrid partDataGrid = this.GetTemplateChild("PART_DataGrid") as DataGrid;
      Panel partSearchPanel = this.GetTemplateChild("PART_SearchPanel") as Panel;

      if (colVM != null && partDataGrid != null)
      {
        GridManager.BuildDataGrid(partDataGrid, colVM,
                                  centerCellStyle, watermarkTextbox,
                                  this.HandleRefreshOnInputKeyInFilter,
                                  partSearchPanel);

        ////Not necessary since the ViewModel creates the CollectionView and looks after the filtering
        ////this.AssignSource(new Binding("Items") { Source = this, Mode = BindingMode.OneWay });
      }
    }
    #endregion Methods
  }
}
