using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace WikipediaApp
{
  public partial class AppShell
  {
    public void CustomizeTitleBar(ApplicationView applicationView)
    {
      EnableExtendedView();
      UpdateTitleBarForExtendedView(applicationView);

      ActualThemeChanged += OnActualThemeChanged;
    }

    private void EnableExtendedView()
    {
      CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

      UpdateTitleBarLayoutForExtendedView(coreTitleBar);

      Window.Current.SetTitleBar(AppTitleBar);

      coreTitleBar.LayoutMetricsChanged += TitleBarLayoutMetricsChanged;
      coreTitleBar.IsVisibleChanged += TitleBarIsVisibleChanged;
    }

    private void TitleBarLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
    {
      UpdateTitleBarLayoutForExtendedView(sender);
    }

    private void TitleBarIsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
    {
      AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateTitleBarLayoutForExtendedView(CoreApplicationViewTitleBar coreTitleBar)
    {
      LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
      RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);

      AppTitleBar.Height = coreTitleBar.Height;

      AppIcon.Source = new BitmapImage
      {
        UriSource = new Uri("ms-appx:///Assets/Logo.png"),
        DecodePixelWidth = Convert.ToInt32(coreTitleBar.Height),
        DecodePixelHeight = Convert.ToInt32(coreTitleBar.Height)
      };
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      UpdateTitleBarForExtendedView(applicationView);
    }

    private void UpdateTitleBarForExtendedView(ApplicationView applicationView)
    {
      var titleBar = applicationView.TitleBar;
      if (titleBar != null)
      {
        var color = ActualTheme == ElementTheme.Light ? Colors.Black : Colors.White;

        titleBar.ButtonForegroundColor = color;
        titleBar.ButtonBackgroundColor = Colors.Transparent;
        titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
      }
    }
  }
}