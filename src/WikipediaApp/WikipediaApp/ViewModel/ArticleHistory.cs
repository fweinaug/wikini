using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WikipediaApp
{
  public static class ArticleHistory
  {
    public static IList<ArticleHead> All { get; } = new ObservableCollection<ArticleHead>();

    public static void AddArticle(Article article)
    {
      var read = new ReadArticle
      {
        Date = DateTime.Now,
        Language = article.Language,
        PageId = article.PageId,
        Title = article.Title,
        Uri = article.Uri
      };

      using (var context = new WikipediaContext())
      {
        context.History.Add(read);

        context.SaveChanges();
      }

      All.Insert(0, read);
    }

    public static async Task Initialize()
    {
      await Task.Run(() =>
      {
        using (var context = new WikipediaContext())
        {
          var history = context.History.OrderByDescending(x => x.Date).ToList();

          history.ForEach(All.Add);
        }
      });
    }
  }
}