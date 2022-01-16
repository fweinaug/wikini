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
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      DataContext = e.Parameter;

      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
      coreTitleBar.LayoutMetricsChanged += TitleBarLayoutMetricsChanged;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
      coreTitleBar.LayoutMetricsChanged -= TitleBarLayoutMetricsChanged;
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
      OpenOrCloseSplitView(nameof(PaneHistoryView));

      SplitViewPaneTabs.SelectedIndex = 1;
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
      OpenOrCloseSplitView(nameof(PaneFavoritesView));

      SplitViewPaneTabs.SelectedIndex = 0;
    }

    private void LanguagesButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(nameof(PaneLanguagesView));

      SplitViewPaneTabs.SelectedIndex = 2;
    }

    private void SplitViewPaneClosed(SplitView sender, object e)
    {
      static void Hide(UIElement element)
      {
        FadeInOutAnimation.Disable(element);
        element.Visibility = Visibility.Collapsed;
      }

      if (PaneLanguagesView != null)
        Hide(PaneLanguagesView);

      if (PaneFavoritesView != null)
        Hide(PaneFavoritesView);

      if (PaneHistoryView != null)
        Hide(PaneHistoryView);

      if (PaneSettingsView != null)
      {
        PaneSettingsView.CloseDialogs();

        Hide(PaneSettingsView);
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
        if (PaneLanguagesView != null)
          PaneLanguagesView.Visibility = Visibility.Collapsed;

        if (PaneFavoritesView != null)
          PaneFavoritesView.Visibility = Visibility.Collapsed;

        if (PaneHistoryView != null)
          PaneHistoryView.Visibility = Visibility.Collapsed;

        if (PaneSettingsView != null)
          PaneSettingsView.Visibility = Visibility.Collapsed;

        FadeInOutAnimation.Enable(control);
        control.Visibility = Visibility.Visible;

        SplitView.IsPaneOpen = true;
      }
      else
      {
        SplitView.IsPaneOpen = false;
      }
    }

    private void PictureOfTheDayStackPanelPointerEntered(object sender, PointerRoutedEventArgs e)
    {
      PictureOfTheDayStackPanel.Opacity = 0.9;
    }

    private void PictureOfTheDayStackPanelPointerExited(object sender, PointerRoutedEventArgs e)
    {
      PictureOfTheDayStackPanel.Opacity = 0.5;
    }
  }
}