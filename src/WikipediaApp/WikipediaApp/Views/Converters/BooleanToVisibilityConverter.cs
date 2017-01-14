using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WikipediaApp
{
  public class BooleanToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var inverted = false;

      if (parameter != null)
      {
        if (!bool.TryParse(parameter.ToString(), out inverted))
          inverted = false;
      }

      if (value is bool && (bool)value == !inverted)
        return Visibility.Visible;

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}