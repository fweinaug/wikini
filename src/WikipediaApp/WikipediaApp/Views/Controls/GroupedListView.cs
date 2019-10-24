using Windows.UI.Xaml.Controls;

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
}
