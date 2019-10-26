using System;
using System.Numerics;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
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
    private readonly DataTemplate paneFavoritesTemplate;
    private readonly DataTemplate paneContentsTemplate;
    private readonly DataTemplate paneLanguagesTemplate;

    public ArticlePage()
    {
      InitializeComponent();

      if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
      {
        SharedShadow.Receivers.Add(ContentGrid);

        SplitViewPaneGrid.Translation += new Vector3(0, 0, 16);
        SearchBar.Translation += new Vector3(0, 0, 16);
      }

      paneHistoryTemplate = (DataTemplate)Resources["HistoryTemplate"];
      paneFavoritesTemplate = (DataTemplate)Resources["FavoritesTemplate"];
      paneContentsTemplate = (DataTemplate)Resources["ContentsTemplate"];
      paneLanguagesTemplate = (DataTemplate)Resources["LanguagesTemplate"];
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
      {
        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        coreTitleBar.LayoutMetricsChanged += TitleBarLayoutMetricsChanged;
        coreTitleBar.IsVisibleChanged += TitleBarIsVisibleChanged;
      }

      DisplayHelper.ActivateDisplay();

      DataContext = e.Parameter;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      DisplayHelper.ReleaseDisplay();

      Settings.WriteLastArticle(null);

      if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
      {
        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        coreTitleBar.LayoutMetricsChanged -= TitleBarLayoutMetricsChanged;
        coreTitleBar.IsVisibleChanged -= TitleBarIsVisibleChanged;
      }
    }

    private void TitleBarLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
    {
      UpdateTitleBar(sender);
    }

    private void TitleBarIsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
    {
      UpdateTitleBar(sender);
    }

    private void UpdateTitleBar(CoreApplicationViewTitleBar sender)
    {
      var height = sender.IsVisible && sender.Height > 0 ? sender.Height + 1 : 0;

      SplitViewPaneGrid.Padding = ArticleView.Margin = new Thickness(0, height, 0, 0);
      AppTitleBarBackground.Height = height;
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
      OpenOrCloseSplitView(paneHistoryTemplate);
    }

    private void HistoryViewArticleClick(object sender, EventArgs e)
    {
      if (SplitView.DisplayMode == SplitViewDisplayMode.Overlay)
        SplitView.IsPaneOpen = false;
    }

    private void FavoritesButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(paneFavoritesTemplate);
    }

    private void FavoritesViewArticleClick(object sender, EventArgs e)
    {
      if (SplitView.DisplayMode == SplitViewDisplayMode.Overlay)
        SplitView.IsPaneOpen = false;
    }

    private void ContentsButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(paneContentsTemplate);
    }

    private void ContentsListViewItemClick(object sender, ItemClickEventArgs e)
    {
      var section = e.ClickedItem as ArticleSection;
      if (string.IsNullOrWhiteSpace(section?.Anchor))
        return;

      if (SplitView.DisplayMode == SplitViewDisplayMode.Overlay)
        SplitView.IsPaneOpen = false;

      ArticleView.ScrollToSection(section);
    }

    private void LangaugesButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(paneLanguagesTemplate);
    }

    private void LanguagesListViewItemClick(object sender, ItemClickEventArgs e)
    {
      if (SplitView.DisplayMode == SplitViewDisplayMode.Overlay)
        SplitView.IsPaneOpen = false;
    }

    private void SplitViewPaneClosed(SplitView sender, object e)
    {
      PaneContentPresenter.ContentTemplate = null;
      PaneContentPresenter.Visibility = Visibility.Visible;

      if (PaneSpeechView != null)
        PaneSpeechView.Visibility = Visibility.Collapsed;

      if (PaneSettingsView != null)
      {
        PaneSettingsView.CloseDialogs();

        PaneSettingsView.Visibility = Visibility.Collapsed;
      }
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

    private void SettingsAppBarButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(nameof(PaneSettingsView));
    }

    private void SpeechButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(nameof(PaneSpeechView));
    }

    private void ClosePaneButtonClick(object sender, RoutedEventArgs e)
    {
      SplitView.IsPaneOpen = false;
    }

    private void ImagesViewLoaded(object sender, RoutedEventArgs e)
    {
      if (ImagesView.Content is Grid grid)
        grid.Padding = SplitViewPaneGrid.Padding;
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
      if (e.Key == VirtualKey.Escape)
      {
        HideSearchBar();

        e.Handled = true;
      }
      else if (e.Key == VirtualKey.Enter)
      {
        if (IsShiftKeyPressed())
          ArticleView.SearchBackward();
        else
          ArticleView.SearchForward();

        e.Handled = true;
      }
    }

    private void SearchBarTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
      ArticleView.Search(SearchBarTextBox.Text);
    }

    private void SearchForwardButtonClick(object sender, RoutedEventArgs e)
    {
      ArticleView.SearchForward();
    }

    private void SearchBackwardButtonClick(object sender, RoutedEventArgs e)
    {
      ArticleView.SearchBackward();
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

    private static bool IsShiftKeyPressed()
    {
      var state = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);

      return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
    }

    private void OpenOrCloseSplitView(string name)
    {
      var control = (UIElement)FindName(name);

      if (!SplitView.IsPaneOpen || control.Visibility == Visibility.Collapsed)
      {
        if (PaneSettingsView != null)
          PaneSettingsView.Visibility = Visibility.Collapsed;

        if (PaneSpeechView != null)
          PaneSpeechView.Visibility = Visibility.Collapsed;

        PaneContentPresenter.Visibility = Visibility.Collapsed;
        PaneContentPresenter.ContentTemplate = null;

        control.Visibility = Visibility.Visible;

        SplitView.IsPaneOpen = true;
      }
      else
      {
        SplitView.IsPaneOpen = false;
      }
    }

    private void OpenOrCloseSplitView(DataTemplate template)
    {
      if (!SplitView.IsPaneOpen || PaneContentPresenter.Visibility == Visibility.Collapsed ||
          PaneContentPresenter.ContentTemplate != template)
      {
        if (PaneSettingsView != null)
          PaneSettingsView.Visibility = Visibility.Collapsed;

        if (PaneSpeechView != null)
          PaneSpeechView.Visibility = Visibility.Collapsed;

        PaneContentPresenter.Visibility = Visibility.Visible;
        PaneContentPresenter.ContentTemplate = template;

        SplitView.IsPaneOpen = true;
      }
      else
      {
        SplitView.IsPaneOpen = false;
      }
    }
  }
}