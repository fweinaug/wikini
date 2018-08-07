using System;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.EntityFrameworkCore;
using Microsoft.HockeyApp;

namespace WikipediaApp
{
  sealed partial class App : Application
  {
    private AppShell appShell = null;
    private Settings settings = null;

    public new static App Current
    {
      get { return (App)Application.Current; }
    }

    public AppShell AppShell
    {
      get { return appShell; }
    }

    public Settings Settings
    {
      get { return settings; }
    }

    public App()
    {
      HockeyClient.Current.Configure("4fa03478a9044a33980678a0ebca5859");

      InitializeComponent();

      using (var database = new WikipediaContext())
      {
        database.Database.Migrate();
      }
    }

    public bool InDarkMode()
    {
      var appTheme = settings.AppTheme;
      var darkMode = appTheme == 2 ||
                     appTheme == 0 && RequestedTheme == ApplicationTheme.Dark;

      return darkMode;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      applicationView.SetPreferredMinSize(new Size(320, 320));

      var titleBar = applicationView.TitleBar;
      if (titleBar != null)
      {
        var titleBarColor = (Color)Current.Resources["SystemAccentColor"];

        titleBar.BackgroundColor = titleBarColor;
        titleBar.ButtonBackgroundColor = titleBarColor;
      }

      var shell = Window.Current.Content as AppShell;

      if (shell == null)
      {
        shell = new AppShell();
        shell.AppFrame.NavigationFailed += OnNavigationFailed;

        Window.Current.Content = shell;

        this.appShell = shell;
        this.settings = (Settings)Resources["Settings"];
      }

      if (e.TileId != "App")
      {
        var article = TileManager.ParseArguments(e.Arguments);

        shell.ShowArticle(article);
      }

      if (!e.PrelaunchActivated)
      {
        if (shell.AppFrame.Content == null)
        {
          // Navigate to the first page
          shell.AppFrame.Navigate(typeof(MainPage), null);
        }

        Window.Current.Activate();
      }
    }

    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }
  }
}