namespace log4netLib.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Adjusts a given double value into a result double value by
    /// 'adding' a parameter string to the bound value.
    /// 
    /// The parameter string can be '-n' to subtract or 'n' to add any value
    /// to the bound value.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    internal class AdjustValueConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Adjusts a bound value by the value indicated in parameter.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return value;

            double adjust = 0;
            if (parameter != null)
                double.TryParse(parameter as string, out adjust);

            double coord = 0;
            double.TryParse(value.ToString(), out coord);

            double res = coord + adjust;

            return res >= 0 ? res : coord;
        }

        /// <summary>
        /// Just returns the bound value (conversion back is not implemented)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}
