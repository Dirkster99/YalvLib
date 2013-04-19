namespace ModernYalv.Settings
{
  using System;
  using System.Windows;
  using System.Xml.Serialization;
  using ModernYalv.Interfaces;

  /// <summary>
  /// Simple wrapper class for allowing windows to persist their
  /// position, height, and width between user sessions in Properties.Default...
  /// 
  /// Todo: The storing of Positions must be extended to store collections of
  /// window positions rather than just one window (because it works only for
  /// one window otherwise ...)
  /// </summary>
  [Serializable]
  [XmlRoot(ElementName = "ControlPos", IsNullable = true)]
  public class ViewPosSize
  {
    #region fields
    private double mX, mY, mWidth, mHeight;
    private bool mIsMaximized;
    #endregion fields

    #region constructors
    /// <summary>
    /// Standard class constructor
    /// </summary>
    public ViewPosSize()
    {
      this.mX = 0;
      this.mY = 0;
      this.mWidth = 0;
      this.mHeight = 0;
      this.mIsMaximized = false;
      this.DefaultConstruct = true;
    }

    /// <summary>
    /// Class cosntructor from coordinates of control
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public ViewPosSize(int x, int y, int width, int height, bool isMaximized = false)
    {
      this.mX = x;
      this.mY = y;
      this.mWidth = width;
      this.mHeight = height;
      this.mIsMaximized = isMaximized;
      this.DefaultConstruct = false;
    }

    /// <summary>
    /// Convinience constructor from window object
    /// </summary>
    /// <param name="c"></param>
    public ViewPosSize(Window c)
      : this()
    {
      if (c != null)
      {
        this.X = c.Left;
        this.Y = c.Top;
        this.Width = c.Width;
        this.Height = c.Height;
        this.DefaultConstruct = false;

        this.IsMaximized = (c.WindowState == WindowState.Maximized);
      }
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copy"></param>
    public ViewPosSize(ViewPosSize copy)
      : this()
    {
      if (copy == null)
        return;

      this.mX                = copy.mX;
      this.mY                = copy.mY;
      this.mWidth            = copy.mWidth;
      this.mHeight           = copy.mHeight;
      this.DefaultConstruct  = copy.DefaultConstruct;

      this.mIsMaximized      = copy.mIsMaximized;
    }
    #endregion constructors

    #region properties
    public bool DefaultConstruct { get; set; }

    /// <summary>
    /// Get/set X coordinate of control
    /// </summary>
    [XmlAttribute(AttributeName = "X")]
    public double X
    {
      get
      {
        return this.mX;
      }

      set
      {
        if (this.mX != value)
        {
          this.mX = value;
        }
      }
    }

    /// <summary>
    /// Get/set Y coordinate of control
    /// </summary>
    [XmlAttribute(AttributeName = "Y")]
    public double Y
    {
      get
      {
        return this.mY;
      }

      set
      {
        if (this.mY != value)
        {
          this.mY = value;
        }
      }
    }

    /// <summary>
    /// Get/set width of control
    /// </summary>
    [XmlAttribute(AttributeName = "Width")]
    public double Width
    {
      get
      {
        return this.mWidth;
      }

      set
      {
        if (this.mWidth != value)
        {
          this.mWidth = value;
        }
      }
    }

    /// <summary>
    /// Get/set height of control
    /// </summary>
    [XmlAttribute(AttributeName = "Height")]
    public double Height
    {
      get
      {
        return this.mHeight;
      }

      set
      {
        if (this.mHeight != value)
        {
          this.mHeight = value;
        }
      }
    }

    /// <summary>
    /// Get/set whether view is amximized or not
    /// </summary>
    [XmlAttribute(AttributeName = "IsMaximized")]
    public bool IsMaximized
    {
      get
      {
        return this.mIsMaximized;
      }

      set
      {
        if (this.mIsMaximized != value)
        {
          this.mIsMaximized = value;
        }
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Convinience function to set the position of a view to a valid position
    /// </summary>
    public void SetValidPos()
    {
      // Restore the position with a valid position
      if (this.X < SystemParameters.VirtualScreenLeft)
        this.X = SystemParameters.VirtualScreenLeft;

      if (this.Y < SystemParameters.VirtualScreenTop)
        this.Y = SystemParameters.VirtualScreenTop;
    }

    /// <summary>
    /// Convinience function to set the position, height, and width of a window
    /// according to the values stored in this class
    /// </summary>
    /// <param name="c"></param>
    public void SetPos(IWinSimple c)
    {
      if (c != null)
      {
        c.Left = this.X;
        c.Top = this.Y;
        c.Width = this.Width;
        c.Height = this.Height;

        if (this.IsMaximized == true)
          c.WindowState = WindowState.Maximized;
        else
          c.WindowState = WindowState.Normal;
      }
    }
    #endregion methods
  }
}
