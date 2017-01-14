using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WikipediaApp
{
  public sealed partial class ArticlePage : Page
  {
    private readonly DataTemplate paneHistoryTemplate;
    private readonly DataTemplate paneContentsTemplate;
    private readonly DataTemplate paneLanguagesTemplate;

    public ArticlePage()
    {
      InitializeComponent();

      paneHistoryTemplate = (DataTemplate)Resources["HistoryTemplate"];
      paneContentsTemplate = (DataTemplate)Resources["ContentsTemplate"];
      paneLanguagesTemplate = (DataTemplate)Resources["LanguagesTemplate"];
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      DataContext = e.Parameter;
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

    private void ContentsButtonClick(object sender, RoutedEventArgs e)
    {
      PaneContentPresenter.ContentTemplate = paneContentsTemplate;

      SplitView.IsPaneOpen = true;
    }

    private void ContentsListViewItemClick(object sender, ItemClickEventArgs e)
    {
      var section = e.ClickedItem as ArticleSection;
      if (string.IsNullOrWhiteSpace(section?.Anchor))
        return;

      SplitView.IsPaneOpen = false;

      var anchor = section.Anchor;

      WebView.ScrollToElement(anchor);
    }

    private void LangaugesButtonClick(object sender, RoutedEventArgs e)
    {
      PaneContentPresenter.ContentTemplate = paneLanguagesTemplate;

      SplitView.IsPaneOpen = true;
    }

    private void LanguagesListViewItemClick(object sender, ItemClickEventArgs e)
    {
      SplitView.IsPaneOpen = false;
    }

    private void SplitViewPaneClosed(SplitView sender, object e)
    {
      PaneContentPresenter.ContentTemplate = null;
    }

    private void TopButtonClick(object sender, RoutedEventArgs e)
    {
      WebView.ScrollToTop();
    }
  }
}