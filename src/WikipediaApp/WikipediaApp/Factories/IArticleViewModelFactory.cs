namespace WikipediaApp
{
  public interface IArticleViewModelFactory
  {
    ArticleViewModel GetArticle(Article article);
    ArticleViewModel GetArticle(ArticleHead initialArticle);
  }
}