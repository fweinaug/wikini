using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WikipediaApp
{
  public class CollectionToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var inverted = false;

      if (parameter != null)
      {
        if (!bool.TryParse(parameter.ToString(), out inverted))
          inverted = false;
      }

      var isEmpty = true;

      var collection = value as IEnumerable;
      if (collection != null)
      {
        var enumerator = collection.GetEnumerator();
        isEmpty = !enumerator.MoveNext();
      }

      if (isEmpty == inverted)
        return Visibility.Visible;

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}