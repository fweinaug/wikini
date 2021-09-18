using System;
using Windows.UI.Xaml.Data;

namespace WikipediaApp
{
  public class EnumToBooleanConverter : IValueConverter
  {
    public Type EnumType {  get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if (parameter is string enumValue)
      {
        return value?.ToString() == enumValue;
      }
      throw new ArgumentException("parameter");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      if (parameter is string enumValue)
      {
        return value?.Equals(true) == true ? Enum.Parse(EnumType, enumValue) : null;
      }
      throw new ArgumentException("parameter");
    }
  }
}