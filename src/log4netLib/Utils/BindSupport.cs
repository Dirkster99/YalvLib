namespace log4netLib.Utils
{
  using System.Windows;

  /// <summary>
  /// The dependency property in this class can be bound to a readonly dependency property, such as, ActualWidth.
  /// (normal viewmodel properties cannot be used in this case)
  /// </summary>
  public class BindSupport : DependencyObject
  {
    /// <summary>
    /// Width dependency property
    /// </summary>
    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register("Width",
                                    typeof(double),
                                    typeof(BindSupport),
                                    new UIPropertyMetadata(25.0));

    /// <summary>
    /// Get/set width dependency property
    /// </summary>
    public double Width
    {
      get { return (double)GetValue(WidthProperty); }
      set { SetValue(WidthProperty, value); }
    }
  }
}
