namespace WikipediaApp
{
  public interface IAppSettings
  {
    string Language { get; }

    bool DarkMode { get; }

    ArticleHead ReadLastArticle();
    void WriteLastArticle(ArticleHead value);
  }
}