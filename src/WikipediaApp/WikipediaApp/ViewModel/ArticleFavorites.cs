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

    public static void AddArticle(ArticleHead article)
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

      All.Insert(GetIndexByTitle(favorite), favorite);
    }

    private static int GetIndexByTitle(ArticleHead favorite)
    {
      var index = 0;

      foreach (var article in All)
      {
        var compare = string.Compare(favorite.Title, article.Title, StringComparison.CurrentCulture);

        if (compare > 0)
          ++index;
        else if (compare < 0)
          break;
        else if (string.Compare(favorite.Language, article.Language, StringComparison.CurrentCultureIgnoreCase) > 0)
          ++index;
      }

      return index;
    }

    public static void RemoveArticle(ArticleHead article)
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

    public static bool IsFavorite(ArticleHead article)
    {
      return All.Any(x => x.Language == article.Language && x.PageId == article.PageId);
    }

    public static async Task Initialize()
    {
      await Task.Run(() =>
      {
        using (var context = new WikipediaContext())
        {
          var favorites = context.Favorites.OrderBy(x => x.Title).ThenBy(x => x.Language).ToList();

          favorites.ForEach(All.Add);
        }
      });
    }
  }
}