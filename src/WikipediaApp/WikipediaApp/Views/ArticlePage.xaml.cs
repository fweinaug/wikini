using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
      DisplayHelper.ActivateDisplay();

      DataContext = e.Parameter;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      DataTransferManager.GetForCurrentView().DataRequested -= ArticlePageDataRequested;
      DisplayHelper.ReleaseDisplay();

      Settings.WriteLastArticle(null);
    }

    private void ArticlePageDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
    {
      var article = ArticleView.Article;
      if (article == null)
        return;

      e.Request.Data.Properties.Title = article.Title;
      e.Request.Data.SetWebLink(article.Uri);
    }

    private void ArticleViewArticleChanged(object sender, EventArgs e)
    {
      var article = ArticleView.Article;

      Settings.WriteLastArticle(article);

      HideSearchBar(resetSearch: false);
    }

    private void DefaultCommandBarOpening(object sender, object e)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      if (applicationView.IsFullScreenMode)
      {
        DefaultEnterFullScreenButton.Visibility = Visibility.Collapsed;
        DefaultExitFullScreenButton.Visibility = Visibility.Visible;
      }
      else
      {
        DefaultEnterFullScreenButton.Visibility = Visibility.Visible;
        DefaultExitFullScreenButton.Visibility = Visibility.Collapsed;
      }
    }

    private void NarrowCommandBarOpening(object sender, object e)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      if (applicationView.IsFullScreenMode)
      {
        NarrowEnterFullScreenButton.Visibility = Visibility.Collapsed;
        NarrowExitFullScreenButton.Visibility = Visibility.Visible;
      }
      else
      {
        NarrowEnterFullScreenButton.Visibility = Visibility.Visible;
        NarrowExitFullScreenButton.Visibility = Visibility.Collapsed;
      }
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
      PaneContentPresenter.Visibility = Visibility.Visible;

      if (PaneSpeechView != null)
        PaneSpeechView.Visibility = Visibility.Collapsed;
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

    private void SpeechButtonClick(object sender, RoutedEventArgs e)
    {
      FindName("PaneSpeechView");

      PaneContentPresenter.Visibility = Visibility.Collapsed;
      PaneSpeechView.Visibility = Visibility.Visible;

      SplitView.IsPaneOpen = true;
    }

    private void ImageStatesCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
    {
      if (ImagesView.Visibility == Visibility.Visible)
        ImagesView.Focus(FocusState.Programmatic);
    }

    private void ToggleSearchBarAppBarButtonClick(object sender, RoutedEventArgs e)
    {
      if (SearchBarTranslate.Y > 0)
        ShowSearchBar();
      else
        HideSearchBar();
    }

    private void HideSearchBarButtonClick(object sender, RoutedEventArgs e)
    {
      HideSearchBar();
    }

    private void SearchBarTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
      if (e.Key == Windows.System.VirtualKey.Escape)
      {
        HideSearchBar();
      }
    }

    private void SearchBarTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
      ArticleView.Search(SearchBarTextBox.Text);
    }

    private void HideSearchBarStoryboardCompleted(object sender, object e)
    {
      SearchBarTextBox.IsEnabled = false;

      SearchBarTextBox.TextChanged -= SearchBarTextBoxTextChanged;
    }

    private void ShowSearchBar()
    {
      if (SearchBarTranslate.Y < 50)
        return;

      SearchBarTextBox.IsEnabled = true;
      SearchBarTextBox.Focus(FocusState.Programmatic);

      SearchBarTextBox.TextChanged += SearchBarTextBoxTextChanged;

      HideSearchBarStoryboard.Stop();
      ShowSearchBarStoryboard.Begin();
    }

    private void HideSearchBar(bool resetSearch = true)
    {
      if (SearchBarTranslate.Y > 0)
        return;

      ShowSearchBarStoryboard.Stop();
      HideSearchBarStoryboard.Begin();

      if (!resetSearch)
        SearchBarTextBox.TextChanged -= SearchBarTextBoxTextChanged;

      SearchBarTextBox.Text = string.Empty;
    }

    private void EnterFullScreenButtonClick(object sender, RoutedEventArgs e)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      applicationView.TryEnterFullScreenMode();
    }

    private void ExitFullScreenButtonClick(object sender, RoutedEventArgs e)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      applicationView.ExitFullScreenMode();
    }
  }
}