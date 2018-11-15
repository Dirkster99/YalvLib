namespace YalvLib.Common.Converters
{
    using log4netLib.Enums;
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// If empty string then collapsed
    /// </summary>
    [ValueConversion(typeof(LevelIndex), typeof(Visibility))]
    public class LevelIndexToVisibilityConverter
        : IValueConverter
    {
        /// <summary>
        /// Convert a <seealso cref="LevelIndex"/>
        /// into <seealso cref="Visibility.Collapsed"/> or
        /// <seealso cref="Visibility.Visible"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return Visibility.Collapsed;

            if ((value is LevelIndex) == false)
                return Visibility.Collapsed;

            LevelIndex levelIndex = (LevelIndex)value;

            switch (levelIndex)
            {
                case LevelIndex.NONE:
                case LevelIndex.DEBUG:
                case LevelIndex.INFO:
                case LevelIndex.WARN:
                case LevelIndex.ERROR:
                case LevelIndex.FATAL:
                    return Visibility.Visible;

                default:
                    return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Convert back from Visibility to LevelIndex enum is not supported.
        /// NotImplementedException is thrown.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Convert back from Visibility to LevelIndex enum is not supported.");
        }
    }
}
