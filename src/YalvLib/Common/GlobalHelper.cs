namespace YalvLib.Common
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using YalvLib.Domain;
  using YalvLib.Providers;

  public class GlobalHelper
  {
    #region fields
    public const string LAYOUT_LOG4J = "http://jakarta.apache.org/log4j";

    // Default date time format (used if there is no localized version available)
    private const string DISPLAY_DATETIME_FORMAT = "MMM d, HH:mm:ss.fff";
    #endregion fields

    #region properties
    /// <summary>
    /// Determine the correct date time format for the currently used local
    /// </summary>
    public static string DisplayDateTimeFormat
    {
      get
      {
        string localizedFormat = YalvLib.Strings.Resources.GlobalHelper_DISPLAY_DATETIME_FORMAT;
        return string.IsNullOrWhiteSpace(localizedFormat) ? DISPLAY_DATETIME_FORMAT : localizedFormat;
      }
    }

    public static string GetTimeDelta(DateTime prevDate, DateTime currentDate)
    {
      double delta = (currentDate - prevDate).TotalSeconds;

      ////if (DateTime.Compare(currentDate.Date, prevDate.Date) == 0)
      return string.Format(delta >= 0 ? YalvLib.Strings.Resources.GlobalHelper_getTimeDelta_Positive_Text :
                                        YalvLib.Strings.Resources.GlobalHelper_getTimeDelta_Negative_Text,
                                        delta.ToString(System.Globalization.CultureInfo.GetCultureInfo(YalvLib.Strings.Resources.CultureName)));
      ////else
      ////    return "-";
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Parse a log4net log file via abstract parser Provider class for SQL, Sqlite, XML file etc...
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static IList<LogItem> ParseLogFile(string path)
    {
      IEnumerable<LogItem> result = null;
      try
      {
        AbstractEntriesProvider provider = EntriesProviderFactory.GetProvider();
        result = provider.GetEntries(path);
        return result.ToList();
      }
      catch (Exception ex)
      {
        string message = string.Format(YalvLib.Strings.Resources.GlobalHelper_ParseLogFile_Error_Text, path, ex.Message);

        MessageBox.Show(message, YalvLib.Strings.Resources.GlobalHelper_ParseLogFile_Error_Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);

        return result == null ? new List<LogItem>() : result.ToList();
      }
    }
    #endregion methods
  }
}