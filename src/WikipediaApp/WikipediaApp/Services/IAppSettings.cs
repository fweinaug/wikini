namespace WikipediaApp
{
  public interface IAppSettings
  {
    bool DarkMode { get; }

    ArticleHead ReadLastArticle();
    void WriteLastArticle(ArticleHead value);
  }
}