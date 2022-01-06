using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WikipediaApp
{
  public class GroupedListView : ListView
  {
    private readonly GroupStyle groupStyle = (GroupStyle)App.Current.Resources["ListViewGroupStyle"];

    public GroupedListView()
    {
      GroupStyle.Add(groupStyle);
    }
  }

  public class GroupKeyToTitleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var resourceLoader = ResourceLoader.GetForCurrentView();

      if (value is string s)
      {
        var title = resourceLoader.GetString(s);

        return title;
      }
      
      if (value is DateTime date)
      {
        var diff = DateTime.Today - date;

        if (diff.Days > 0)
          return date.ToShortDateString();

        var title = resourceLoader.GetString("ArticleGroupDateToday");

        return title;
      }

      return value?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}