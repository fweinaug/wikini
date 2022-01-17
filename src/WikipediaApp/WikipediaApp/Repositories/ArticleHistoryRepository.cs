using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WikipediaApp
{
  public class ArticleHistoryRepository : IArticleHistoryRepository
  {
    public ReadArticle AddArticle(ArticleHead article)
    {
      var read = new ReadArticle
      {
        Date = DateTime.Now,
        Language = article.Language,
        PageId = article.PageId,
        Title = article.Title,
        Description = article.Description,
        Uri = article.Uri,
        ThumbnailUri = article.ThumbnailUri
      };

      using (var context = new WikipediaContext())
      {
        context.History.Add(read);

        context.SaveChanges();
      }

      return read;
    }

    public void RemoveArticle(ReadArticle article)
    {
      using (var context = new WikipediaContext())
      {
        var readArticle = context.History.FirstOrDefault(x => x.Id == article.Id);

        if (readArticle != null)
          context.History.Remove(readArticle);

        context.SaveChanges();
      }
    }

    public async Task<List<ReadArticle>> GetHistory()
    {
      using (var context = new WikipediaContext())
      {
        var history = await context.History.OrderByDescending(x => x.Date).ToListAsync();

        return history;
      }
    }

    public async Task Clear()
    {
      using (var context = new WikipediaContext())
      {
        context.RemoveRange(context.History);
        await context.SaveChangesAsync();
      }
    }
  }
}