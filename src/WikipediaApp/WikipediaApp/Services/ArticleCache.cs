using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WikipediaApp
{
  public class ArticleCache<T> where T: ArticleHead
  {
    private readonly IList<T> articles = new ObservableCollection<T>();

    public void AddArticle(T article)
    {
      if (articles.Count == 0 || !articles.Contains(article))
        articles.Add(article);
    }

    public T FindArticle(Uri uri, string language = null, int? pageId = null)
    {
      if (uri != null)
      {
        var article = articles.FirstOrDefault(x => x.Uri == uri);
        if (article != null)
          return article;
      }

      if (!string.IsNullOrEmpty(language) && pageId != null)
      {
        var article = articles.FirstOrDefault(x => x.Language == language && x.PageId == pageId);
        if (article != null)
          return article;
      }

      return null;
    }

    public T FindArticle(string language, int? pageId, string title)
    {
      if (pageId != null)
      {
        var article = articles.FirstOrDefault(x => x.Language == language && x.PageId == pageId);
        if (article != null)
          return article;
      }

      if (!string.IsNullOrEmpty(title))
      {
        var article = articles.FirstOrDefault(x => x.Language == language && x.Title == title);
        if (article != null)
          return article;
      }

      return null;
    }
  }
}