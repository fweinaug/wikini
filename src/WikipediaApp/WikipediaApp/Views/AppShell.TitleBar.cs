using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace WikipediaApp
{
  public partial class AppShell
  {
    public void CustomizeTitleBar(ApplicationView applicationView)
    {
      if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
      {
        EnableExtendedView();
        UpdateTitleBarForExtendedView(applicationView);

        ActualThemeChanged += OnActualThemeChanged;
      }
      else
      {
        UpdateTitleBarForDefaultView(applicationView);
      }
    }
    private void EnableExtendedView()
    {
      CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

      UpdateTitleBarLayoutForExtendedView(coreTitleBar);

      Window.Current.SetTitleBar(AppTitleBar);

      coreTitleBar.LayoutMetricsChanged += TitleBarLayoutMetricsChanged;
      coreTitleBar.IsVisibleChanged += TitleBarIsVisibleChanged;

      AppTitle.Text = Package.Current.DisplayName;
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

    private void UpdateTitleBarForDefaultView(ApplicationView applicationView)
    {
      var titleBar = applicationView.TitleBar;
      if (titleBar != null)
      {
        var titleBarColor = (Color)Resources["SystemAccentColor"];

        titleBar.BackgroundColor = titleBarColor;
        titleBar.ButtonBackgroundColor = titleBarColor;
        titleBar.ButtonForegroundColor = Colors.White;
      }
    }
  }
}