using System;
using Windows.UI.Xaml.Data;

namespace WikipediaApp
{
  public class MillisecondsToMinutesConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var timeSpan = TimeSpan.FromMilliseconds((double)value);
      return timeSpan.ToString("mm\\:ss");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}