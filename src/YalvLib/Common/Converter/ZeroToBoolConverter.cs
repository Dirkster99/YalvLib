namespace YalvLib.Converter
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Windows.Data;
  using System.Windows.Markup;

  /// <summary>
  /// XAML mark up extension to convert a zero count value into a boolean value.
  /// </summary>
  [MarkupExtensionReturnType(typeof(IValueConverter))]
  public class ZeroToBoolConverter : MarkupExtension, IValueConverter
  {
    private static ZeroToBoolConverter converter;

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ZeroToBoolConverter()
    {
    }
  
    /// <summary>
    /// When implemented in a derived class, returns an object that is provided
    /// as the value of the target property for this markup extension.
    /// 
    /// When a XAML processor processes a type node and member value that is a markup extension,
    /// it invokes the ProvideValue method of that markup extension and writes the result into the
    /// object graph or serialization stream. The XAML object writer passes service context to each
    /// such implementation through the serviceProvider parameter.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new ZeroToBoolConverter();
      }
  
      return converter;
    }

    #region IValueConverter
    /// <summary>
    /// Zero to visibility conversion method
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return false;

      if (value is int)
      {
        if ((int)value == 0)
          return false;
      }

      return true;
    }

    /// <summary>
    /// Visibility to Zero conversion method (is disabled and will throw an exception when invoked)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }
    #endregion IValueConverter
  }
}
