using System;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WikipediaApp
{
  sealed partial class App : Application
  {
    private IServiceProvider serviceProvider;
    private IAppSettings appSettings;
    private Settings settings;

    public new static App Current
    {
      get { return (App)Application.Current; }
    }

    public static IServiceProvider Services
    {
      get { return ((App)Current).serviceProvider; }
    }

    public AppShell AppShell { get; private set; }

    public App()
    {
#if !DEBUG
      AppCenter.Start("{Your App Secret}", typeof(Analytics), typeof(Crashes));
#endif

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

      InitApp(article, false);
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
      ArticleHead article = null;

      if (e.TileId != "App")
      {
        article = TileManager.ParseArguments(e.Arguments);
      }

      InitApp(article, true, e.PrelaunchActivated);
    }

    private void InitApp(ArticleHead article, bool launched, bool prelaunchActivated = false)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      applicationView.SetPreferredMinSize(new Size(320, 320));

      var shell = Window.Current.Content as AppShell;

      if (shell == null)
      {
        serviceProvider = ConfigureServices();
        appSettings = serviceProvider.GetService<IAppSettings>();
        settings = new Settings(serviceProvider.GetService<IUserSettings>());

        Resources.Add("Settings", settings);

        shell = new AppShell();
        shell.AppFrame.NavigationFailed += OnNavigationFailed;

        shell.CustomizeTitleBar(applicationView);

        Window.Current.Content = shell;

        AppShell = shell;
      }

      if (article != null)
      {
        shell.ShowArticle(article);
      }
      else if (launched && !settings.StartHome)
      {
        article = appSettings.ReadLastArticle();
        if (article != null)
          shell.ShowArticle(article);
      }

      if (!prelaunchActivated)
      {
        if (shell.AppFrame.Content == null)
        {
          // Navigate to the first page
          shell.AppFrame.Navigate(typeof(MainPage), shell.ViewModel.MainPage);
        }

        Window.Current.Activate();
      }
    }

    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    private static IServiceProvider ConfigureServices()
    {
      var provider = new ServiceCollection()
        .AddTransient<IWikipediaService, WikipediaService>()
        .AddTransient<IWikipediaContentBuilder, WikipediaHtmlBuilder>()
        .AddTransient<IDeviceService, DeviceService>()
        .AddTransient<IDialogService, DialogService>()
        .AddTransient<INavigationService, NavigationService>()
        .AddTransient<IGeolocationService, GeolocationService>()
        .AddTransient<IShareManager, ShareManager>()
        .AddSingleton<IDisplay, Display>()
        .AddSingleton<ISystemSettingProvider, SystemSettingProvider>()
        .AddSingleton<IAppSettings, AppSettings>()
        .AddSingleton<IUserSettings, RoamingUserSettings>()
        .AddSingleton<IArticleViewModelFactory, ArticleViewModelFactory>()
        .AddSingleton<AppShellViewModel>()
        .AddSingleton<FavoritesViewModel>()
        .AddSingleton<HistoryViewModel>()
        .AddSingleton<MainPageViewModel>()
        .AddTransient<MapPageViewModel>()
        .AddTransient<ArticlePageViewModel>()
        .AddTransient<ArticleFlyoutViewModel>()
        .AddTransient<LanguagesViewModel>()
        .AddTransient<SearchViewModel>()
        .AddTransient<SettingsViewModel>()
        .AddTransient<PictureOfTheDayViewModel>()
        .BuildServiceProvider(true);

      return provider;
    }
  }
}