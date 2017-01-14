using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      InitializeComponent();
    }

    private void LanguagesButtonClick(object sender, RoutedEventArgs e)
    {
      SplitView.IsPaneOpen = true;
    }

    private void SelectedLanguageButtonClick(object sender, RoutedEventArgs e)
    {
      SplitView.IsPaneOpen = true;
    }

    private void LanguagesListViewItemClick(object sender, ItemClickEventArgs e)
    {
      SplitView.IsPaneOpen = false;
    }
  }
}