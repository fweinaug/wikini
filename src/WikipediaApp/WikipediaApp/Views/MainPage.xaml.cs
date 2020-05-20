using System;
using System.Numerics;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

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
        SearchShadow.Receivers.Add(Background);

        SplitViewPaneGrid.Translation += new Vector3(0, 0, 16);
        SearchBox.Translation += new Vector3(0, 0, 32);

        SearchBox.PointerEntered += SearchBoxPointerEntered;
        SearchBox.PointerExited += SearchBoxPointerExited;
      }

      paneHistoryTemplate = (DataTemplate)Resources["HistoryTemplate"];
      paneFavoritesTemplate = (DataTemplate)Resources["FavoritesTemplate"];
      paneLanguagesTemplate = (DataTemplate)Resources["LanguagesTemplate"];
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
      {
        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        coreTitleBar.LayoutMetricsChanged += TitleBarLayoutMetricsChanged;
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
      {
        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        coreTitleBar.LayoutMetricsChanged -= TitleBarLayoutMetricsChanged;
      }
    }

    private void TitleBarLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
    {
      SplitViewPaneGrid.Padding = new Thickness(0, sender.Height, 0, 0);
    }

    private void SearchBoxPointerEntered(object sender, PointerRoutedEventArgs e)
    {
      SearchBox.Shadow = SearchShadow;
    }

    private void SearchBoxPointerExited(object sender, PointerRoutedEventArgs e)
    {
      SearchBox.Shadow = null;
    }

    private void HistoryButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(paneHistoryTemplate);

      SplitViewPaneTabs.SelectedIndex = 2;
    }

    private void HistoryViewArticleClick(object sender, EventArgs e)
    {
      if (SplitView.DisplayMode == SplitViewDisplayMode.Overlay)
        SplitView.IsPaneOpen = false;
    }

    private void FavoritesViewArticleClick(object sender, EventArgs e)
    {
      if (SplitView.DisplayMode == SplitViewDisplayMode.Overlay)
        SplitView.IsPaneOpen = false;
    }

    private void LanguagesViewLanguageClick(object sender, EventArgs e)
    {
      if (SplitView.DisplayMode == SplitViewDisplayMode.Overlay)
        SplitView.IsPaneOpen = false;
    }

    private void FavoritesButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(paneFavoritesTemplate);

      SplitViewPaneTabs.SelectedIndex = 1;
    }

    private void LanguagesButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(paneLanguagesTemplate);

      SplitViewPaneTabs.SelectedIndex = 0;
    }

    private void SplitViewPaneClosed(SplitView sender, object e)
    {
      if (PaneContentPresenter != null)
      {
        PaneContentPresenter.ContentTemplate = null;
        PaneContentPresenter.Visibility = Visibility.Visible;
      }

      if (PaneSettingsView != null)
      {
        PaneSettingsView.CloseDialogs();

        PaneSettingsView.Visibility = Visibility.Collapsed;
      }
    }

    private void SettingsAppBarButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(nameof(PaneSettingsView));

      SplitViewPaneTabs.SelectedIndex = 3;
    }

    private void ClosePaneButtonClick(object sender, RoutedEventArgs e)
    {
      SplitView.IsPaneOpen = false;
    }

    private void OpenOrCloseSplitView(string name)
    {
      var control = (UIElement)FindName(name);

      if (!SplitView.IsPaneOpen || control.Visibility == Visibility.Collapsed)
      {
        if (PaneSettingsView != null)
          PaneSettingsView.Visibility = Visibility.Collapsed;

        if (PaneContentPresenter != null)
        {
          PaneContentPresenter.Visibility = Visibility.Collapsed;
          PaneContentPresenter.ContentTemplate = null;
        }

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
      var paneContentPresenter = (ContentPresenter)FindName("PaneContentPresenter");

      if (!SplitView.IsPaneOpen || paneContentPresenter.Visibility == Visibility.Collapsed ||
          paneContentPresenter.ContentTemplate != template)
      {
        if (PaneSettingsView != null)
          PaneSettingsView.Visibility = Visibility.Collapsed;

        paneContentPresenter.Visibility = Visibility.Visible;
        paneContentPresenter.ContentTemplate = template;

        SplitView.IsPaneOpen = true;
      }
      else
      {
        SplitView.IsPaneOpen = false;
      }
    }
  }
}