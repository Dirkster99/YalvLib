using YalvLib.Model;

namespace YalvLib.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using YalvLib.Domain;
    using YalvLib.Providers;
    using YalvLib.ViewModel;

    /// <summary>
    /// Class to supply global utility properties and methods
    /// </summary>
    public class GlobalHelper
    {
        #region fields
        /// <summary>
        /// XML namespace for log4net logger XML format
        /// </summary>
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
                string localizedFormat = log4netLib.Strings.Resources.GlobalHelper_DISPLAY_DATETIME_FORMAT;
                return string.IsNullOrWhiteSpace(localizedFormat) ? DISPLAY_DATETIME_FORMAT : localizedFormat;
            }
        }

        /// <summary>
        /// Helper method to return a delta data item between two <seealso cref="DateTime"/> parameters.
        /// </summary>
        /// <param name="prevDate"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public static string GetTimeDelta(DateTime prevDate, DateTime currentDate)
        {
            double delta = (currentDate - prevDate).TotalSeconds;

            ////if (DateTime.Compare(currentDate.Date, prevDate.Date) == 0)
            return string.Format(delta >= 0 ? log4netLib.Strings.Resources.GlobalHelper_getTimeDelta_Positive_Text :
                                              log4netLib.Strings.Resources.GlobalHelper_getTimeDelta_Negative_Text,
                                              delta.ToString(System.Globalization.CultureInfo.GetCultureInfo(log4netLib.Strings.Resources.CultureName)));
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
        public static IList<LogEntry> ParseLogFile(string path)
        {
            IEnumerable<LogEntry> result = null;
            try
            {
                AbstractEntriesProviderBase provider = EntriesProviderFactory.GetProvider();
                result = provider.GetEntries(path);
                return result.ToList();
            }
            catch (Exception ex)
            {
                string message = string.Format(log4netLib.Strings.Resources.GlobalHelper_ParseLogFile_Error_Text, path, ex.Message);

                MessageBox.Show(message, log4netLib.Strings.Resources.GlobalHelper_ParseLogFile_Error_Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);

                return result == null ? new List<LogEntry>() : result.ToList();
            }
        }
        #endregion methods
    }
}