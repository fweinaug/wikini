using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WikipediaApp
{
  public static class ArticleHistory
  {
    public static IList<Article> Session { get; } = new ObservableCollection<Article>();

    public static void AddArticle(Article article)
    {
      if (Session.Count == 0 || !Session.Contains(article) && Session[0].Uri != article.Uri)
        Session.Insert(0, article);
    }
  }
}