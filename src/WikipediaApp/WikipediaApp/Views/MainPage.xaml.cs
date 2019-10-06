using System;
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

        SplitViewPaneGrid.Translation += new Vector3(0, 0, 16);
      }

      paneHistoryTemplate = (DataTemplate)Resources["HistoryTemplate"];
      paneFavoritesTemplate = (DataTemplate)Resources["FavoritesTemplate"];
      paneLanguagesTemplate = (DataTemplate)Resources["LanguagesTemplate"];
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

    private void FavoritesViewArticleClick(object sender, EventArgs e)
    {
      if (SplitView.DisplayMode == SplitViewDisplayMode.Overlay)
        SplitView.IsPaneOpen = false;
    }

    private void FavoritesButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(paneFavoritesTemplate);
    }

    private void LanguagesButtonClick(object sender, RoutedEventArgs e)
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

      if (PaneSettingsView != null)
      {
        PaneSettingsView.CloseDialogs();

        PaneSettingsView.Visibility = Visibility.Collapsed;
      }
    }

    private void SettingsAppBarButtonClick(object sender, RoutedEventArgs e)
    {
      OpenOrCloseSplitView(nameof(PaneSettingsView));
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