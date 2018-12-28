using System;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
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

    protected override void OnActivated(IActivatedEventArgs e)
    {
      ArticleHead article = null;

      if (e.Kind == ActivationKind.Protocol)
      {
        var args = (IProtocolActivatedEventArgs)e;

        article = TimelineManager.ParseArticle(args.Uri);
      }

      InitApp(article);
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
      ArticleHead article = null;

      if (e.TileId != "App")
      {
        article = TileManager.ParseArguments(e.Arguments);
      }
      else
      {
        article = Settings.ReadLastArticle();
      }

      InitApp(article, e.PrelaunchActivated);
    }

    private void InitApp(ArticleHead article, bool prelaunchActivated = false)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      applicationView.SetPreferredMinSize(new Size(320, 320));

      var shell = Window.Current.Content as AppShell;

      if (shell == null)
      {
        shell = new AppShell();
        shell.AppFrame.NavigationFailed += OnNavigationFailed;

        shell.CustomizeTitleBar(applicationView);

        Window.Current.Content = shell;

        this.appShell = shell;
        this.settings = (Settings)Resources["Settings"];
      }

      if (article != null)
        shell.ShowArticle(article);

      if (!prelaunchActivated)
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