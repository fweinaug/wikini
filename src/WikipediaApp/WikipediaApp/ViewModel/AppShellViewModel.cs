using System.Threading.Tasks;

namespace WikipediaApp
{
  public class AppShellViewModel : ViewModelBase
  {
    private readonly INavigationService navigationService;

    private readonly MainPageViewModel mainPage;
    private readonly FavoritesViewModel favoritesViewModel;
    private readonly HistoryViewModel historyViewModel;

    public MainPageViewModel MainPage => mainPage;

    public AppShellViewModel(INavigationService navigationService, MainPageViewModel mainPageViewModel, FavoritesViewModel favoritesViewModel, HistoryViewModel historyViewModel)
    {
      this.navigationService = navigationService;
      this.mainPage = mainPageViewModel;
      this.favoritesViewModel = favoritesViewModel;
      this.historyViewModel = historyViewModel;
    }

    public void ShowArticle(ArticleHead article)
    {
      navigationService.ShowArticle(article);
    }

    public override async Task Initialize()
    {
      await favoritesViewModel.Initialize();
      await historyViewModel.Initialize();
      await mainPage.Initialize();
    }
  }
}