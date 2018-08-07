using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WikipediaApp
{
  public static class ArticleCache
  {
    public static IList<Article> Session { get; } = new ObservableCollection<Article>();

    public static void AddArticle(Article article)
    {
      if (Session.Count == 0 || !Session.Contains(article))
        Session.Add(article);
    }

    public static Article GetArticle(Uri uri, string language = null, int? pageId = null)
    {
      if (uri != null)
      {
        var article = Session.FirstOrDefault(x => x.Uri == uri);
        if (article != null)
          return article;
      }

      if (!string.IsNullOrEmpty(language) && pageId != null)
      {
        var article = Session.FirstOrDefault(x => x.Language == language && x.PageId == pageId);
        if (article != null)
          return article;
      }

      return null;
    }
  }
}