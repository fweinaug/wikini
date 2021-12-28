using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace WikipediaApp
{
  public partial class AppViewModel : ViewModelBase
  {
    private readonly WikipediaService wikipediaService = new WikipediaService();
    private readonly NavigationService navigationService = new NavigationService();
    private readonly DialogService dialogService = new DialogService();

    private bool isBusy = false;

    private RelayCommand showHomePageCommand = null;
    private RelayCommand showRandomArticleCommand = null;
    private RelayCommand showMapCommand = null;
    private RelayCommand<ArticleHead> showArticleCommand = null;

    private IList<Language> languages = null;
    private Language language = null;
    private RelayCommand<Language> changeLanguageCommand = null;

    public SearchViewModel Search { get; }

    public bool IsBusy
    {
      get { return isBusy; }
      private set { SetProperty(ref isBusy, value); }
    }

    public ICommand ShowHomePageCommand
    {
      get { return showHomePageCommand ?? (showHomePageCommand = new RelayCommand(ShowHomePage)); }
    }

    public ICommand ShowRandomArticleCommand
    {
      get { return showRandomArticleCommand ?? (showRandomArticleCommand = new RelayCommand(ShowRandomArticle)); }
    }

    public ICommand ShowMapCommand
    {
      get { return showMapCommand ?? (showMapCommand = new RelayCommand(ShowMap)); }
    }

    public ICommand ShowArticleCommand
    {
      get { return showArticleCommand ?? (showArticleCommand = new RelayCommand<ArticleHead>(ShowArticle)); }
    }

    public IList<ArticleGroup> History
    {
      get { return ArticleHistory.All; }
    }

    public IList<ArticleHead> Favorites
    {
      get { return ArticleFavorites.All; }
    }

    public IList<Language> Languages
    {
      get { return languages; }
      private set { SetProperty(ref languages, value); }
    }

    public Language Language
    {
      get { return language; }
      private set
      {
        var currentLanguage = language;

        if (SetProperty(ref language, value))
        {
          language.IsActive = true;

          if (currentLanguage != null)
            currentLanguage.IsActive = false;

          Search.Language = language;
        }
      }
    }

    public ICommand ChangeLanguageCommand
    {
      get { return changeLanguageCommand ?? (changeLanguageCommand = new RelayCommand<Language>(ChangeLanguage)); }
    }

    public PictureOfTheDayViewModel PictureOfTheDay { get; }

    public AppViewModel()
    {
      Search = new SearchViewModel();
      PictureOfTheDay = new PictureOfTheDayViewModel(wikipediaService);
    }

    private async void ShowHomePage()
    {
      IsBusy = true;

      var article = await wikipediaService.GetMainPage(language.Code);

      if (article != null)
        navigationService.ShowArticle(article);
      else
        dialogService.ShowLoadingError();

      IsBusy = false;
    }

    private async void ShowRandomArticle()
    {
      IsBusy = true;

      var article = await wikipediaService.GetRandomArticle(language.Code);

      if (article != null)
        navigationService.ShowArticle(article);
      else
        dialogService.ShowLoadingError();

      IsBusy = false;
    }

    private void ShowMap()
    {
      navigationService.ShowMap(Language.Code);
    }

    public void ShowArticle(ArticleHead article)
    {
      navigationService.ShowArticle(article);
    }

    private void ChangeLanguage(Language language)
    {
      if (Language == language)
        return;

      Settings.Current.SearchLanguage = language.Code;

      Language = language;
    }

    public override async Task Initialize()
    {
      await ArticleLanguages.Initialize();

      var language = Settings.Current.SearchLanguage;

      Languages = ArticleLanguages.All;
      Language = Languages.FirstOrDefault(x => x.Code == language) ?? Languages.First(x => x.Code == Settings.DefaultSearchLanguage);

      await ArticleFavorites.Initialize();
      await ArticleHistory.Initialize();

      if (Settings.Current.StartPictureOfTheDay)
        PictureOfTheDay.Today();
    }
  }
}