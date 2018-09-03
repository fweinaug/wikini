using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace WikipediaApp
{
  public partial class AppShell
  {
    public void CustomizeTitleBar(ApplicationView applicationView)
    {
      UpdateTitleBar(applicationView);

      ActualThemeChanged += OnActualThemeChanged;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      UpdateTitleBar(applicationView);
    }

    private void UpdateTitleBar(ApplicationView applicationView)
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
  }
}