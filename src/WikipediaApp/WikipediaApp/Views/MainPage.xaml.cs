using System.Numerics;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public sealed partial class MainPage : Page
  {
    private readonly DataTemplate paneHistoryTemplate;
    private readonly DataTemplate paneFavoritesTemplate;
    private readonly DataTemplate paneLanguagesTemplate;

    public MainPage()
    {
      InitializeComponent();

      if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
      {
        SharedShadow.Receivers.Add(ContentGrid);

        PaneContentPresenter.Translation += new Vector3(0, 0, 16);
      }

      paneHistoryTemplate = (DataTemplate)Resources["HistoryTemplate"];
      paneFavoritesTemplate = (DataTemplate)Resources["FavoritesTemplate"];
      paneLanguagesTemplate = (DataTemplate)Resources["LanguagesTemplate"];
    }

    private void HistoryButtonClick(object sender, RoutedEventArgs e)
    {
      PaneContentPresenter.ContentTemplate = paneHistoryTemplate;

      SplitView.IsPaneOpen = true;
    }

    private void HistoryListViewItemClick(object sender, ItemClickEventArgs e)
    {
      SplitView.IsPaneOpen = false;
    }

    private void FavoritesButtonClick(object sender, RoutedEventArgs e)
    {
      PaneContentPresenter.ContentTemplate = paneFavoritesTemplate;

      SplitView.IsPaneOpen = true;
    }

    private void FavoritesListViewItemClick(object sender, ItemClickEventArgs e)
    {
      SplitView.IsPaneOpen = false;
    }

    private void LanguagesButtonClick(object sender, RoutedEventArgs e)
    {
      PaneContentPresenter.ContentTemplate = paneLanguagesTemplate;

      SplitView.IsPaneOpen = true;
    }

    private void LanguagesListViewItemClick(object sender, ItemClickEventArgs e)
    {
      SplitView.IsPaneOpen = false;
    }
  }
}