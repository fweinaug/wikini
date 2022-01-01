using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WikipediaApp
{
  public static class ArticleLanguages
  {
    public static IList<LanguageViewModel> All { get; } = new ObservableCollection<LanguageViewModel>();

    private static readonly List<FavoriteLanguage> Favorites = new List<FavoriteLanguage>();

    public static void AddFavorite(LanguageViewModel language)
    {
      var favorite = new FavoriteLanguage
      {
        Date = DateTime.Now,
        Code = language.Code
      };

      using (var context = new WikipediaContext())
      {
        context.Languages.Add(favorite);

        context.SaveChanges();
      }

      Favorites.Add(favorite);

      language.IsFavorite = true;
    }

    public static void RemoveFavorite(LanguageViewModel language)
    {
      var favorite = Favorites.FirstOrDefault(x => x.Code == language.Code);
      if (favorite == null)
        return;

      using (var context = new WikipediaContext())
      {
        context.Languages.Remove(favorite);

        context.SaveChanges();
      }

      Favorites.Remove(favorite);

      language.IsFavorite = false;
    }

    public static bool IsFavorite(string code)
    {
      return Favorites.Any(x => x.Code == code);
    }

    public static async Task Initialize()
    {
      var allLanguages = await LanguagesReader.GetLanguages();

      var languages = allLanguages.Select((language, index) => new LanguageViewModel(language, index));
      foreach (var language in languages)
      {
        All.Add(language);
      }

      using (var context = new WikipediaContext())
      {
        var favorites = await context.Languages.ToListAsync();

        foreach (var favorite in favorites)
        {
          Favorites.Add(favorite);

          var language = languages.SingleOrDefault(x => x.Code == favorite.Code);
          if (language != null)
          {
            language.IsFavorite = true;
          }
        }
      }
    }
  }
}