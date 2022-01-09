namespace WikipediaApp
{
  public class ArticleViewModelFactory : IArticleViewModelFactory
  {
    private readonly IWikipediaService wikipediaService;
    private readonly IDialogService dialogService;
    private readonly INavigationService navigationService;
    private readonly IUserSettings userSettings;
    private readonly IShareManager shareManager;

    public ArticleViewModelFactory(IWikipediaService wikipediaService, IDialogService dialogService, INavigationService navigationService, IUserSettings userSettings, IShareManager shareManager)
    {
      this.wikipediaService = wikipediaService;
      this.dialogService = dialogService;
      this.navigationService = navigationService;
      this.userSettings = userSettings;
      this.shareManager = shareManager;
    }

    public ArticleViewModel GetArticle(ArticleHead initialArticle)
    {
      return new ArticleViewModel(initialArticle, wikipediaService, navigationService, userSettings, shareManager);
    }

    public ArticleViewModel GetArticle(Article article)
    {
      var languagesViewModel = new ArticleLanguagesViewModel();
      languagesViewModel.UpdateLanguages(article.Languages, ArticleLanguages.Favorites);

      return new ArticleViewModel(article, wikipediaService, navigationService, userSettings, shareManager, languagesViewModel);
    }
  }
}