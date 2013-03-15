namespace YalvLib.Domain
{
  using System;
  using YalvLib.Common;

  public enum CellAlignment
  {
    DEFAULT = 0,
    CENTER = 1
  }

  [Serializable]
  public class ColumnItem : BindableObject
  {
    #region fields
    private string mColumnFilterValue = string.Empty;
    #endregion fields

    #region constructor
    public ColumnItem()
    {
      this.ColumnFilterValue = 
      this.FilterControlName = string.Empty;
    }

    public ColumnItem(string field, double? minWidth, double? width)
      : this(field, minWidth, width, CellAlignment.DEFAULT, string.Empty, field)
    {
    }

    public ColumnItem(string field, double? minWidth, double? width, CellAlignment align)
      : this(field, minWidth, width, align, string.Empty, field)
    {
    }

    public ColumnItem(string field, double? minWidth, double? width, CellAlignment align, string stringFormat)
      : this(field, minWidth, width, align, stringFormat, field)
    {
    }

    public ColumnItem(string field, double? minWidth, double? width, CellAlignment align, string stringFormat, string header)
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
    public string Header { get; set; }

    public string Field { get; set; }

    public string FilterControlName { get; set; }

    public string StringFormat { get; set; }

    public double? MinWidth { get; set; }

    public double? Width { get; set; }

    public CellAlignment Alignment { get; set; }

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
    #endregion properties
  }
}
