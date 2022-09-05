using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WikipediaApp
{
  public class AppShellViewModel : ObservableObject
  {
    private readonly INavigationService navigationService;
    private readonly IUserSettings userSettings;

    private readonly MainPageViewModel mainPageViewModel;
    private readonly FavoritesViewModel favoritesViewModel;
    private readonly HistoryViewModel historyViewModel;

    public MainPageViewModel MainPage => mainPageViewModel;

    public int AppTheme
    {
      get => userSettings.Get<int>(UserSettingsKey.AppTheme);
      set => userSettings.Set(UserSettingsKey.AppTheme, value);
    }

    public AppShellViewModel(INavigationService navigationService, IUserSettings userSettings, MainPageViewModel mainPageViewModel, FavoritesViewModel favoritesViewModel, HistoryViewModel historyViewModel)
    {
      this.navigationService = navigationService;
      this.userSettings = userSettings;
      this.mainPageViewModel = mainPageViewModel;
      this.favoritesViewModel = favoritesViewModel;
      this.historyViewModel = historyViewModel;
    }

    public void ShowArticle(ArticleHead article)
    {
      navigationService.ShowArticle(article);
    }

    public async Task Initialize()
    {
      await favoritesViewModel.Initialize();
      await historyViewModel.Initialize();

      userSettings.SettingSet += (sender, settingKey) =>
      {
        if (settingKey == UserSettingsKey.AppTheme)
          OnPropertyChanged(nameof(AppTheme));
      };
    }
  }
}