using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace WikipediaApp
{
  public class MainPageViewModel : ObservableObject
  {
    private readonly IWikipediaService wikipediaService;
    private readonly INavigationService navigationService;
    private readonly IDialogService dialogService;
    private readonly IArticleLanguagesRepository articleLanguagesRepository;
    private readonly IUserSettings userSettings;

    private bool isBusy = false;

    private AsyncRelayCommand loadCommand = null;
    private RelayCommand showHomePageCommand = null;
    private RelayCommand showRandomArticleCommand = null;
    private RelayCommand showMapCommand = null;
    private RelayCommand<ArticleHead> showArticleCommand = null;

    private LanguageViewModel language = null;
    private RelayCommand<LanguageViewModel> changeLanguageCommand = null;

    public bool IsBusy
    {
      get { return isBusy; }
      private set { SetProperty(ref isBusy, value); }
    }

    public bool SidebarInline
    {
      get { return userSettings.Get<bool>(UserSettingsKey.SplitViewInline); }
      set { userSettings.Set(UserSettingsKey.SplitViewInline, value); }
    }

    public AsyncRelayCommand LoadCommand
    {
      get { return loadCommand ?? (loadCommand = new AsyncRelayCommand(Initialize)); }
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

    public LanguagesViewModel Languages { get; }

    public LanguageViewModel Language
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
      get { return changeLanguageCommand ?? (changeLanguageCommand = new RelayCommand<LanguageViewModel>(ChangeLanguage)); }
    }

    public SearchViewModel Search { get; }

    public PictureOfTheDayViewModel PictureOfTheDay { get; }

    public MainPageViewModel(IWikipediaService wikipediaService, INavigationService navigationService, IDialogService dialogService, IArticleLanguagesRepository articleLanguagesRepository, IUserSettings userSettings, SearchViewModel searchViewModel, PictureOfTheDayViewModel pictureOfTheDayViewModel, LanguagesViewModel languagesViewModel)
    {
      this.wikipediaService = wikipediaService;
      this.navigationService = navigationService;
      this.dialogService = dialogService;
      this.articleLanguagesRepository = articleLanguagesRepository;
      this.userSettings = userSettings;

      Languages = languagesViewModel;
      Search = searchViewModel;
      PictureOfTheDay = pictureOfTheDayViewModel;
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

    private void ChangeLanguage(LanguageViewModel language)
    {
      if (Language == language)
        return;

      userSettings.Set(UserSettingsKey.SearchLanguage, language.Code);

      Language = language;
    }

    private async Task Initialize()
    {
      if (Language == null)
      {
        var languages = await articleLanguagesRepository.GetAll();
        var favorites = await articleLanguagesRepository.GetFavorites();

        Languages.UpdateLanguages(languages, favorites);

        Language = Languages.GetLanguage(userSettings.Get<string>(UserSettingsKey.SearchLanguage))
          ?? Languages.GetLanguage(UserSettingsKey.Default<string>(UserSettingsKey.SearchLanguage));
      }

      if (userSettings.Get<bool>(UserSettingsKey.StartPictureOfTheDay) && PictureOfTheDay.ThumbnailUri == null)
        PictureOfTheDay.Today();

      userSettings.SettingSet += (sender, settingKey) =>
      {
        if (settingKey == UserSettingsKey.SplitViewInline)
          OnPropertyChanged(nameof(SidebarInline));
      };
    }
  }
}