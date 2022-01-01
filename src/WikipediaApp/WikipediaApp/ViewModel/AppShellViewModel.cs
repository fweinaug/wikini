using System.Threading.Tasks;

namespace WikipediaApp
{
  public class AppShellViewModel : ViewModelBase
  {
    private readonly INavigationService navigationService;

    private readonly MainPageViewModel mainPage;

    public MainPageViewModel MainPage => mainPage;

    public AppShellViewModel(INavigationService navigationService, MainPageViewModel mainPageViewModel)
    {
      this.navigationService = navigationService;
      this.mainPage = mainPageViewModel;
    }

    public void ShowArticle(ArticleHead article)
    {
      navigationService.ShowArticle(article);
    }

    public override async Task Initialize()
    {
      await ArticleLanguages.Initialize();
      await ArticleFavorites.Initialize();
      await ArticleHistory.Initialize();

      await mainPage.Initialize();
    }
  }
}