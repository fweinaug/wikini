using Windows.ApplicationModel.DataTransfer;
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

      DataTransferManager.GetForCurrentView().DataRequested += ArticlePageDataRequested;

      DataContext = e.Parameter;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      DataTransferManager.GetForCurrentView().DataRequested -= ArticlePageDataRequested;
    }

    private void ArticlePageDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
    {
      var article = ArticleView.Article;
      if (article == null)
        return;

      e.Request.Data.Properties.Title = article.Title;
      e.Request.Data.SetWebLink(article.Uri);
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

      ArticleView.ScrollToSection(section);
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
      ArticleView.ScrollToTop();
    }

    private void BackButtonClick(object sender, RoutedEventArgs e)
    {
      ArticleView.GoBack();
    }

    private void ForwardButtonClick(object sender, RoutedEventArgs e)
    {
      ArticleView.GoForward();
    }

    private void ShareButtonClick(object sender, RoutedEventArgs e)
    {
      DataTransferManager.ShowShareUI();
    }

    private void ImageStatesCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
    {
      if (ImagesView.Visibility == Visibility.Visible)
        ImagesView.Focus(FocusState.Programmatic);
    }
  }
}