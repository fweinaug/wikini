using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikipediaApp
{
  public partial class AppViewModel : ViewModelBase
  {
    private readonly WikipediaService wikipediaService = new WikipediaService();
    private readonly NavigationService navigationService = new NavigationService();
    private readonly DialogService dialogService = new DialogService();
    private readonly DeviceService deviceService = new DeviceService();

    private bool isBusy = false;

    private Command showHomePageCommand = null;
    private Command showRandomArticleCommand = null;
    private Command showMapCommand = null;
    private Command<ArticleHead> showArticleCommand = null;

    private IList<Language> languages = null;
    private Language language = null;
    private Command<Language> changeLanguageCommand = null;

    public bool IsBusy
    {
      get { return isBusy; }
      private set { SetProperty(ref isBusy, value); }
    }

    public ICommand ShowHomePageCommand
    {
      get { return showHomePageCommand ?? (showHomePageCommand = new Command(ShowHomePage)); }
    }

    public ICommand ShowRandomArticleCommand
    {
      get { return showRandomArticleCommand ?? (showRandomArticleCommand = new Command(ShowRandomArticle)); }
    }

    public ICommand ShowMapCommand
    {
      get { return showMapCommand ?? (showMapCommand = new Command(ShowMap)); }
    }

    public ICommand ShowArticleCommand
    {
      get { return showArticleCommand ?? (showArticleCommand = new Command<ArticleHead>(ShowArticle)); }
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
        }
      }
    }

    public ICommand ChangeLanguageCommand
    {
      get { return changeLanguageCommand ?? (changeLanguageCommand = new Command<Language>(ChangeLanguage)); }
    }

    public PictureOfTheDay PictureOfTheDay { get; }

    public AppViewModel()
    {
      PictureOfTheDay = new PictureOfTheDay(wikipediaService);
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
      ClearSearch();

      navigationService.ShowArticle(article);
    }

    private void ChangeLanguage(Language language)
    {
      if (Language == language)
        return;

      Settings.Current.SearchLanguage = language.Code;

      Language = language;
      Search();
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