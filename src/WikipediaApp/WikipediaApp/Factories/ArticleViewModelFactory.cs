namespace WikipediaApp
{
  public class ArticleViewModelFactory : IArticleViewModelFactory
  {
    private readonly IWikipediaService wikipediaService;
    private readonly IDialogService dialogService;
    private readonly INavigationService navigationService;
    private readonly IShareManager shareManager;

    public ArticleViewModelFactory(IWikipediaService wikipediaService, IDialogService dialogService, INavigationService navigationService, IShareManager shareManager)
    {
      this.wikipediaService = wikipediaService;
      this.dialogService = dialogService;
      this.navigationService = navigationService;
      this.shareManager = shareManager;
    }

    public ArticleViewModel GetArticle(ArticleHead initialArticle)
    {
      return new ArticleViewModel(initialArticle, wikipediaService, navigationService, shareManager);
    }

    public ArticleViewModel GetArticle(Article article)
    {
      return new ArticleViewModel(article, wikipediaService, navigationService, shareManager);
    }
  }
}