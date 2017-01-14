using System;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
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
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
      var shell = Window.Current.Content as AppShell;

      if (shell == null)
      {
        // TODO: Am Ende entfernen
        ApplicationLanguages.PrimaryLanguageOverride = "en-US";

        shell = new AppShell();
        shell.AppFrame.NavigationFailed += OnNavigationFailed;

        Window.Current.Content = shell;

        this.appShell = shell;
        this.settings = (Settings)Resources["Settings"];
      }

      if (!e.PrelaunchActivated)
      {
        if (shell.AppFrame.Content == null)
        {
          // Navigate to the first page
          shell.AppFrame.Navigate(typeof(MainPage), e.Arguments);
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