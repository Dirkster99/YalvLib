namespace YalvLib.Common.Converters
{
  using System;
  using System.Windows;
  using System.Windows.Data;
  using System.Windows.Media;

  /// <summary>
  /// Convert a log4net message level (Info, Warn, Debug) into a color code.
  /// </summary>
  public class LevelToSolidColorConverter : IValueConverter
  {
    private SolidColorBrush debugColor = Application.Current.FindResource("DebugLevelColor") as SolidColorBrush;
    private SolidColorBrush infoColor = Application.Current.FindResource("InfoLevelColor") as SolidColorBrush;
    private SolidColorBrush warnColor = Application.Current.FindResource("WarnLevelColor") as SolidColorBrush;
    private SolidColorBrush errorColor = Application.Current.FindResource("ErrorLevelColor") as SolidColorBrush;
    private SolidColorBrush fatalColor = Application.Current.FindResource("FatalLevelColor") as SolidColorBrush;

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (null == value)
        return Brushes.Transparent;

      int levelIndex = (int)value;
      switch (levelIndex)
      {
        case 1:
          return this.debugColor ?? Brushes.Transparent;

        case 2:
          return this.infoColor ?? Brushes.Transparent;

        case 3:
          return this.warnColor ?? Brushes.Transparent;

        case 4:
          return this.errorColor ?? Brushes.Transparent;

        case 5:
          return this.fatalColor ?? Brushes.Transparent;

        default:
          return Brushes.Transparent;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }
  }
}
