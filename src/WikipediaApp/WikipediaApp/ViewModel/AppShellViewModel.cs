using System.Threading.Tasks;

namespace WikipediaApp
{
  public class AppShellViewModel : ViewModelBase
  {
    private readonly INavigationService navigationService;

    private readonly MainPageViewModel mainPage;
    private readonly FavoritesViewModel favoritesViewModel;

    public MainPageViewModel MainPage => mainPage;

    public AppShellViewModel(INavigationService navigationService, MainPageViewModel mainPageViewModel, FavoritesViewModel favoritesViewModel)
    {
      this.navigationService = navigationService;
      this.mainPage = mainPageViewModel;
      this.favoritesViewModel = favoritesViewModel;
    }

    public void ShowArticle(ArticleHead article)
    {
      navigationService.ShowArticle(article);
    }

    public override async Task Initialize()
    {
      await ArticleLanguages.Initialize();
      await ArticleHistory.Initialize();

      await favoritesViewModel.Initialize();
      await mainPage.Initialize();
    }
  }
}