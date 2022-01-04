using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WikipediaApp
{
  public static class ArticleLanguages
  {
    public static List<FavoriteLanguage> Favorites { get; } = new();

    public static void AddFavorite(string code)
    {
      var favorite = new FavoriteLanguage
      {
        Date = DateTime.Now,
        Code = code
      };

      using (var context = new WikipediaContext())
      {
        context.Languages.Add(favorite);

        context.SaveChanges();
      }

      Favorites.Add(favorite);
    }

    public static void RemoveFavorite(string code)
    {
      using (var context = new WikipediaContext())
      {
        var favorite = context.Languages.FirstOrDefault(x => x.Code == code);

        if (favorite != null)
          context.Languages.Remove(favorite);

        context.SaveChanges();
      }

      var language = Favorites.FirstOrDefault(favorite => favorite.Code == code);
      if (language != null)
        Favorites.Remove(language);
    }

    public static async Task<List<Language>> GetAll()
    {
      var languages = await LanguagesReader.GetLanguages();

      return languages;
    }

    public static async Task<List<FavoriteLanguage>> GetFavorites()
    {
      using (var context = new WikipediaContext())
      {
        var favorites = await context.Languages.ToListAsync();

        Favorites.Clear();
        Favorites.AddRange(favorites);

        return favorites;
      }
    }
  }
}