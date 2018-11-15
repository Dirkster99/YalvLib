namespace YalvLib.Common.Converters
{
  using System;
  using System.Windows.Data;
  using System.Windows.Media;
  using YalvLib.Model;

  /// <summary>
  /// Convert a levelindex (Info, Warn, Debug) into a localizable string.
  /// </summary>
  [ValueConversion(typeof(LevelIndex), typeof(string))]
  public class LevelIndexTypeToStringConverter : IValueConverter
  {
    /// <summary>
    /// Convert from <seealso cref="LevelIndex"/> enum into human readable string
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
          return Strings.Resources.LevelIndex_None;

        case LevelIndex.DEBUG:
          return Strings.Resources.LevelIndex_Debug;

        case LevelIndex.INFO:
          return Strings.Resources.LevelIndex_Info;

        case LevelIndex.WARN:
          return Strings.Resources.LevelIndex_Warning;

        case LevelIndex.ERROR:
          return Strings.Resources.LevelIndex_Error;

        case LevelIndex.FATAL:
          return Strings.Resources.LevelIndex_FatalErorr;

        default:
          throw new NotImplementedException(levelIndex.ToString());
      }
    }

    /// <summary>
    /// Conversion from string to LevelIndex enum is not supported (throws NotImplementedException).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException("Conversion from string to LevelIndex enum is not supported");
    }
  }
}
