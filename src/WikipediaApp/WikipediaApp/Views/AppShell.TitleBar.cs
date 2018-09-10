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
        UpdateTitleBarAdvanced(applicationView);

        ActualThemeChanged += OnActualThemeChanged;
      }
      else
      {
        UpdateTitleBarDefault(applicationView);
      }
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      UpdateTitleBarAdvanced(applicationView);
    }

    private void UpdateTitleBarAdvanced(ApplicationView applicationView)
    {
      var titleBar = applicationView.TitleBar;
      if (titleBar != null)
      {
        var titleBarColor = (Color)Resources["SystemAccentColor"];
        var themeColor = ActualTheme == ElementTheme.Light ? Colors.White : Colors.Black;

        titleBar.BackgroundColor = titleBarColor;
        titleBar.ButtonBackgroundColor = titleBarColor;
        titleBar.ButtonForegroundColor = Colors.White;

        titleBar.InactiveBackgroundColor = themeColor;
        titleBar.ButtonInactiveBackgroundColor = themeColor;
      }
    }

    private void UpdateTitleBarDefault(ApplicationView applicationView)
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