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
    public class LevelToSolidColorConverter : DependencyObject, IValueConverter
    {
        #region fields
        /// <summary>
        /// Backing store of the debug color level dependency property.
        /// </summary>
        public static readonly DependencyProperty DebugColorProperty =
            DependencyProperty.Register("DebugColor", typeof(SolidColorBrush),
                typeof(LevelToSolidColorConverter), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the information color level dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoColorProperty =
            DependencyProperty.Register("InfoColor", typeof(SolidColorBrush),
                typeof(LevelToSolidColorConverter), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the warn color level dependency property.
        /// </summary>
        public static readonly DependencyProperty WarnColorProperty =
            DependencyProperty.Register("WarnColor", typeof(SolidColorBrush),
                typeof(LevelToSolidColorConverter), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the error color level dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorColorProperty =
            DependencyProperty.Register("ErrorColor", typeof(SolidColorBrush),
                typeof(LevelToSolidColorConverter), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the fatal color level dependency property.
        /// </summary>
        public static readonly DependencyProperty FatalColorProperty =
            DependencyProperty.Register("FatalColor", typeof(SolidColorBrush),
                typeof(LevelToSolidColorConverter), new PropertyMetadata(null));
        #endregion fields

        #region properties
        /// <summary>
        /// Gets/sets dependency property for the debug level background color.
        /// </summary>
        public SolidColorBrush DebugColor
        {
            get { return (SolidColorBrush)GetValue(DebugColorProperty); }
            set { SetValue(DebugColorProperty, value); }
        }

        /// <summary>
        /// Gets/sets dependency property for the info level background color.
        /// </summary>
        public SolidColorBrush InfoColor
        {
            get { return (SolidColorBrush)GetValue(InfoColorProperty); }
            set { SetValue(InfoColorProperty, value); }
        }

        /// <summary>
        /// Gets/sets dependency property for the warning level background color.
        /// </summary>
        public SolidColorBrush WarnColor
        {
            get { return (SolidColorBrush)GetValue(WarnColorProperty); }
            set { SetValue(WarnColorProperty, value); }
        }

        /// <summary>
        /// Gets/sets dependency property for the error level background color.
        /// </summary>
        public SolidColorBrush ErrorColor
        {
            get { return (SolidColorBrush)GetValue(ErrorColorProperty); }
            set { SetValue(ErrorColorProperty, value); }
        }

        /// <summary>
        /// Gets/sets dependency property for the fatal level background color.
        /// </summary>
        public SolidColorBrush FatalColor
        {
            get { return (SolidColorBrush)GetValue(FatalColorProperty); }
            set { SetValue(FatalColorProperty, value); }
        }
        #endregion properties

        #region methods
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
                    return this.DebugColor ?? Brushes.Transparent;

                case LevelIndex.INFO:
                    return this.InfoColor ?? Brushes.Transparent;

                case LevelIndex.WARN:
                    return this.WarnColor ?? Brushes.Transparent;

                case LevelIndex.ERROR:
                    return this.ErrorColor ?? Brushes.Transparent;

                case LevelIndex.FATAL:
                    return this.FatalColor ?? Brushes.Transparent;

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
        #endregion methods
    }
}
