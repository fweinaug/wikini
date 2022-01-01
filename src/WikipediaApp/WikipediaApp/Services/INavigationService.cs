using System;

namespace WikipediaApp
{
  public interface INavigationService
  {
    void OpenInBrowser(Uri uri);
    void ShowArticle(ArticleHead article);
    void ShowMap(string language);
  }
}