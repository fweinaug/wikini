using System.Threading.Tasks;

namespace WikipediaApp
{
  public class AppShellViewModel : ViewModelBase
  {
    private readonly INavigationService navigationService = new NavigationService();

    private readonly MainPageViewModel mainPage = new MainPageViewModel();

    public MainPageViewModel MainPage => mainPage;

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