using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikipediaApp
{
  public class AppViewModel : ViewModelBase
  {
    private readonly WikipediaService wikipediaService = new WikipediaService();
    private readonly NavigationService navigationService = new NavigationService();
    private readonly DialogService dialogService = new DialogService();
    private readonly DeviceService deviceService = new DeviceService();

    private bool isBusy = false;

    private Command showSettingsCommand = null;
    private Command showHomePageCommand = null;
    private Command showRandomArticleCommand = null;
    private Command<ArticleHead> showArticleCommand = null;

    private IList<Language> languages = null;
    private Language language = null;
    private Command<Language> changeLanguageCommand = null;

    private string searchTerm = null;
    private Command searchCommand = null;
    private IList<ArticleHead> searchResults = null;

    public bool IsBusy
    {
      get { return isBusy; }
      private set { SetProperty(ref isBusy, value); }
    }

    public ICommand ShowSettingsCommand
    {
      get { return showSettingsCommand ?? (showSettingsCommand = new Command(ShowSettings)); }
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

    public string SearchTerm
    {
      get { return searchTerm; }
      set
      {
        if (SetProperty(ref searchTerm, value))
        {
          searchCommand?.RaiseCanExecuteChanged();

          if (Settings.Current.SearchRestricted || deviceService.IsInternetConnectionUnrestricted())
            Search();
        }
      }
    }

    public ICommand SearchCommand
    {
      get { return searchCommand ?? (searchCommand = new Command(Search, CanSearch)); }
    }

    public IList<ArticleHead> SearchResults
    {
      get { return searchResults; }
      private set { SetProperty(ref searchResults, value); }
    }

    private void ShowSettings()
    {
      navigationService.ShowSettings();
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

    private void ShowArticle(ArticleHead article)
    {
      navigationService.ShowArticle(article);
    }

    private void ChangeLanguage(Language language)
    {
      Settings.Current.SearchLanguage = language.Code;

      Language = language;
      SearchResults = null;
    }

    private async void Search()
    {
      await Task.Delay(100);

      if (string.IsNullOrEmpty(searchTerm))
        SearchResults = null;
      else
        SearchResults = await wikipediaService.Search(searchTerm, language.Code);
    }

    private bool CanSearch()
    {
      return !string.IsNullOrEmpty(searchTerm);
    }

    public override async void Initialize()
    {
      var language = Settings.Current.SearchLanguage;

      Languages = await wikipediaService.GetLanguages();
      Language = Languages.FirstOrDefault(x => x.Code == language) ?? Languages.First(x => x.Code == Settings.DefaultSearchLanguage);
    }
  }
}