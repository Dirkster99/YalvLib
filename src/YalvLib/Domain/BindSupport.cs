namespace YalvLib.Domain
{
  using System.Windows;

  public class BindSupport : DependencyObject
  {
    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register("Width", typeof(double), typeof(BindSupport), new UIPropertyMetadata(25.0));

    public double Width
    {
      get { return (double)GetValue(WidthProperty); }
      set { SetValue(WidthProperty, value); }
    }
  }
}
