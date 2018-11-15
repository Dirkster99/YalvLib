namespace YalvLib.Common
{
    using System;

    /// <summary>
    /// Class to supply global utility properties and methods
    /// </summary>
    public class GlobalHelper
    {
        #region fields
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
    }
}