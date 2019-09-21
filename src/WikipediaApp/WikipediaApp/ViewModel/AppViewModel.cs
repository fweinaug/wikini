using System.Collections.Generic;
using System.Linq;
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
      set { SetProperty(ref languages, value); }
    }

    public Language Language
    {
      get { return language; }
      private set { SetProperty(ref language, value); }
    }

    public ICommand ChangeLanguageCommand
    {
      get { return changeLanguageCommand ?? (changeLanguageCommand = new Command<Language>(ChangeLanguage)); }
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
      Search();
    }

    public override async void Initialize()
    {
      var language = Settings.Current.SearchLanguage;

      Languages = await wikipediaService.GetLanguages();
      Language = Languages.FirstOrDefault(x => x.Code == language) ?? Languages.First(x => x.Code == Settings.DefaultSearchLanguage);

      await ArticleHistory.Initialize();
      await ArticleFavorites.Initialize();
    }
  }
}