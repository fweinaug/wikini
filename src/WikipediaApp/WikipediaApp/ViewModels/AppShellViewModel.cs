using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace WikipediaApp
{
  public class AppShellViewModel : ObservableObject
  {
    private readonly INavigationService navigationService;

    private readonly MainPageViewModel mainPageViewModel;
    private readonly FavoritesViewModel favoritesViewModel;
    private readonly HistoryViewModel historyViewModel;

    public MainPageViewModel MainPage => mainPageViewModel;

    public AppShellViewModel(INavigationService navigationService, MainPageViewModel mainPageViewModel, FavoritesViewModel favoritesViewModel, HistoryViewModel historyViewModel)
    {
      this.navigationService = navigationService;
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
    }
  }
}