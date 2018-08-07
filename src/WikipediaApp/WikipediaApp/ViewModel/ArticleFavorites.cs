using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WikipediaApp
{
  public static class ArticleFavorites
  {
    public static IList<ArticleHead> All { get; } = new ObservableCollection<ArticleHead>();

    public static void AddArticle(Article article)
    {
      var favorite = new FavoriteArticle
      {
        Date = DateTime.Now,
        Language = article.Language,
        PageId = article.PageId,
        Title = article.Title,
        Uri = article.Uri
      };

      using (var context = new WikipediaContext())
      {
        context.Favorites.Add(favorite);

        context.SaveChanges();
      }

      All.Insert(0, favorite);
    }

    public static void RemoveArticle(Article article)
    {
      if (All.FirstOrDefault(x => x.Language == article.Language && x.PageId == article.PageId) is FavoriteArticle favorite)
      {
        using (var context = new WikipediaContext())
        {
          context.Favorites.Remove(favorite);

          context.SaveChanges();
        }

        All.Remove(favorite);
      }
    }

    public static bool IsFavorite(Article article)
    {
      return All.Any(x => x.Language == article.Language && x.PageId == article.PageId);
    }

    public static async Task Initialize()
    {
      await Task.Run(() =>
      {
        using (var context = new WikipediaContext())
        {
          var favorites = context.Favorites.OrderByDescending(x => x.Date).ToList();

          favorites.ForEach(All.Add);
        }
      });
    }
  }
}