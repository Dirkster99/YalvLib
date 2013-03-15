namespace YalvLib.Common.Converters
{
  using System;
  using System.Windows.Data;

  public class MultiValueEqualsToBoolConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return values[0].Equals(values[1]);
    }

    public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      return value as object[];
    }
  }
}