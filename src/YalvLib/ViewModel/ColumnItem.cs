namespace YalvLib.ViewModel
{
  using System;
  using YalvLib.Common;
  using YalvLib.Domain;
  using YalvLib.ViewModel.Common;

  /// <summary>
  /// Indicate whether cells in a column are to be arranged by their default or center.
  /// </summary>
  public enum CellAlignment
  {
    /// <summary>
    /// Apply default alignment in column.
    /// </summary>
    DEFAULT = 0,

    /// <summary>
    /// Center cell contents in a datagrid column.
    /// </summary>
    CENTER = 1
  }

  /// <summary>
  /// Objects of this class store layout data about a column in a data grid
  /// </summary>
  [Serializable]
  public class ColumnItem : BindableObject
  {
    #region fields
    private string mColumnFilterValue = string.Empty;
    private bool mIsColumnVisible = true;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard constructor
    /// </summary>
    public ColumnItem()
    {
      this.ColumnFilterValue = 
      this.FilterControlName = string.Empty;

      this.ActualWidth = new BindSupport();
      this.Width = this.MinWidth = 25;
    }

    /// <summary>
    /// Parameterized standard constructor
    /// </summary>
    /// <param name="field"></param>
    /// <param name="minWidth"></param>
    /// <param name="width"></param>
    public ColumnItem(string field, double minWidth, double width)
      : this(field, minWidth, width, CellAlignment.DEFAULT, string.Empty, field)
    {
    }

    /// <summary>
    /// Parameterized standard constructor
    /// </summary>
    /// <param name="field"></param>
    /// <param name="minWidth"></param>
    /// <param name="width"></param>
    /// <param name="align"></param>
    public ColumnItem(string field, double minWidth, double width, CellAlignment align)
      : this(field, minWidth, width, align, string.Empty, field)
    {
    }

    /// <summary>
    /// Parameterized standard constructor
    /// </summary>
    /// <param name="field"></param>
    /// <param name="minWidth"></param>
    /// <param name="width"></param>
    /// <param name="align"></param>
    /// <param name="stringFormat"></param>
    public ColumnItem(string field, double minWidth, double width, CellAlignment align, string stringFormat)
      : this(field, minWidth, width, align, stringFormat, field)
    {
    }

    /// <summary>
    /// Parameterized standard constructor
    /// </summary>
    /// <param name="field"></param>
    /// <param name="minWidth"></param>
    /// <param name="width"></param>
    /// <param name="align"></param>
    /// <param name="stringFormat"></param>
    /// <param name="header"></param>
    public ColumnItem(string field, double minWidth, double width, CellAlignment align, string stringFormat, string header)
      : this()
    {
      this.Field = field;
      this.MinWidth = minWidth;
      this.Width = width;
      this.Alignment = align;
      this.StringFormat = stringFormat;
      this.Header = header;
    }
    #endregion constructor

    #region events
    /// <summary>
    /// This event is raised when a text character is keyed into a filter textbox
    /// </summary>
    public event EventHandler UpdateColumnFilter;
    #endregion events

    #region properties
    /// <summary>
    /// Column header label string
    /// </summary>
    public string Header { get; set; }

    /// <summary>
    /// Field name of data source
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Name of textbox that is used to filter this column.
    /// This field is used to update the column filter when the user
    /// types/edits the textbox content.
    /// </summary>
    public string FilterControlName { get; set; }

    /// <summary>
    /// Get/set string that can be used to filter the view by the contents of this column
    /// </summary>
    public string ColumnFilterValue
    {
      get
      {
        return this.mColumnFilterValue;
      }

      set
      {
        if (this.mColumnFilterValue != value)
        {
          this.mColumnFilterValue = value;
          this.RaisePropertyChanged("ColumnFilterValue");

          if (this.UpdateColumnFilter != null)
            this.UpdateColumnFilter(this, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Get/set string format for dates (and such data types) in localized context. 
    /// </summary>
    public string StringFormat { get; set; }

    /// <summary>
    /// Get/set minimum width of this column.
    /// </summary>
    public double MinWidth { get; set; }

    /// <summary>
    /// Get/set actual width of this column.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Get property to bind the actual width of a column to
    /// (this readonly dp requires a dependency property which in turn
    /// requires an inheritance from DependencyObject)
    /// </summary>
    [System.Xml.Serialization.XmlIgnore]
    public BindSupport ActualWidth { get; private set; }
    
    /// <summary>
    /// Get/set cell alignment for cells displayed in this column.
    /// </summary>
    public CellAlignment Alignment { get; set; }

    /// <summary>
    /// Get/set whether column is visible or not.
    /// </summary>
    public bool IsColumnVisible
    {
      get
      {
        return this.mIsColumnVisible;
      }

      set
      {
        if (this.mIsColumnVisible != value)
        {
          this.mIsColumnVisible = value;
          this.RaisePropertyChanged("IsColumnVisible");
        }
      }
    }
    #endregion properties
  }
}
