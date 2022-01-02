using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WikipediaApp
{
  public static class ArticleFavorites
  {
    public static FavoriteArticle AddArticle(ArticleHead article)
    {
      var favorite = new FavoriteArticle
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
        context.Favorites.Add(favorite);

        context.SaveChanges();
      }

      return favorite;
    }

    public static void RemoveArticle(ArticleHead article)
    {
      using (var context = new WikipediaContext())
      {
        var favorite = context.Favorites.FirstOrDefault(x => x.Language == article.Language && x.PageId == article.PageId);

        if (favorite != null)
          context.Favorites.Remove(favorite);

        context.SaveChanges();
      }
    }

    public static async Task<List<FavoriteArticle>> GetFavorites()
    {
      using (var context = new WikipediaContext())
      {
        var favorites = await context.Favorites.OrderBy(x => x.Title).ThenBy(x => x.Language).ToListAsync();

        return favorites;
      }
    }
  }
}