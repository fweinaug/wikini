namespace WikipediaApp
{
  public class ArticleViewModelFactory : IArticleViewModelFactory
  {
    private readonly IWikipediaService wikipediaService;
    private readonly IDialogService dialogService;
    private readonly INavigationService navigationService;
    private readonly IArticleLanguagesRepository articleLanguagesRepository;
    private readonly IUserSettings userSettings;
    private readonly IShareManager shareManager;

    public ArticleViewModelFactory(IWikipediaService wikipediaService, IDialogService dialogService, INavigationService navigationService, IArticleLanguagesRepository articleLanguagesRepository, IUserSettings userSettings, IShareManager shareManager)
    {
      this.wikipediaService = wikipediaService;
      this.dialogService = dialogService;
      this.navigationService = navigationService;
      this.articleLanguagesRepository = articleLanguagesRepository;
      this.userSettings = userSettings;
      this.shareManager = shareManager;
    }

    public ArticleViewModel GetArticle(ArticleHead initialArticle)
    {
      return new ArticleViewModel(initialArticle, wikipediaService, navigationService, userSettings, shareManager);
    }

    public ArticleViewModel GetArticle(Article article)
    {
      var languagesViewModel = new ArticleLanguagesViewModel(articleLanguagesRepository);
      languagesViewModel.UpdateLanguages(article.Languages, articleLanguagesRepository.Favorites);

      return new ArticleViewModel(article, wikipediaService, navigationService, userSettings, shareManager, languagesViewModel);
    }
  }
}