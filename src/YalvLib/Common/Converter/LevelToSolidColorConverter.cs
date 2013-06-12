namespace YalvLib.Common.Converters
{
  using System;
  using System.Windows;
  using System.Windows.Data;
  using System.Windows.Media;
  using YalvLib.Model;

  /// <summary>
  /// Convert a log4net message level (Info, Warn, Debug) into a color code.
  /// </summary>
  [ValueConversion(typeof(LevelIndex), typeof(SolidColorBrush))]
  public class LevelToSolidColorConverter : IValueConverter
  {
    private SolidColorBrush debugColor = Application.Current.FindResource("DebugLevelColor") as SolidColorBrush;
    private SolidColorBrush infoColor  = Application.Current.FindResource("InfoLevelColor") as SolidColorBrush;
    private SolidColorBrush warnColor  = Application.Current.FindResource("WarnLevelColor") as SolidColorBrush;
    private SolidColorBrush errorColor = Application.Current.FindResource("ErrorLevelColor") as SolidColorBrush;
    private SolidColorBrush fatalColor = Application.Current.FindResource("FatalLevelColor") as SolidColorBrush;

    /// <summary>
    /// Convert <seealso cref="LevelIndex"/> enum value into a color value based on a corresponding resource.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (null == value)
        return Brushes.Transparent;

      if ((value is LevelIndex) == false)
        return Brushes.Transparent;

      LevelIndex levelIndex = (LevelIndex)value;

      switch (levelIndex)
      {
        case LevelIndex.NONE:
          return Brushes.Transparent;

        case LevelIndex.DEBUG:
          return this.debugColor ?? Brushes.Transparent;

        case LevelIndex.INFO:
          return this.infoColor ?? Brushes.Transparent;

        case LevelIndex.WARN:
          return this.warnColor ?? Brushes.Transparent;

        case LevelIndex.ERROR:
          return this.errorColor ?? Brushes.Transparent;

        case LevelIndex.FATAL:
          return this.fatalColor ?? Brushes.Transparent;

        default:
          throw new NotImplementedException(levelIndex.ToString());
      }
    }

    /// <summary>
    /// Conversion back from color code into <seealso cref="LevelIndex"/> enum value
    /// is not supported (not implenented exception is thrown).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException("Conversion from color code to LevelIndex enum value is not supported.");
    }
  }
}
