using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace WikipediaApp
{
  public class ArticleLanguagesViewModel
  {
    private readonly LanguageCollection languages = new LanguageCollection();

    public LanguageCollection All
    {
      get { return languages; }
    }

    public ArticleLanguagesViewModel()
    {
      WeakReferenceMessenger.Default.Register<ArticleLanguagesViewModel, AddLanguageToFavorites>(this, (_, message) =>
      {
        AddFavorite(message.Code);
      });
      WeakReferenceMessenger.Default.Register<ArticleLanguagesViewModel, RemoveLanguageFromFavorites>(this, (_, message) =>
      {
        RemoveFavorite(message.Code);
      });
    }

    public void UpdateLanguages(IEnumerable<ArticleLanguage> languages, IEnumerable<FavoriteLanguage> favorites)
    {
      var viewModels = languages.Select((language, index) => new ArticleLanguageViewModel(language, index)
      {
        IsFavorite = favorites.Any(favorite => favorite.Code == language.Code)
      });

      this.languages.UpdateLanguages(viewModels);
    }

    private void AddFavorite(string code)
    {
      var language = languages.GetLanguage(code);
      if (language == null)
        return;

      ArticleLanguages.AddFavorite(code);

      languages.AddFavorite(language);

      WeakReferenceMessenger.Default.Send(new LanguageIsFavoriteChanged(code, true));
    }

    public void RemoveFavorite(string code)
    {
      var language = languages.GetLanguage(code);
      if (language == null)
        return;

      ArticleLanguages.RemoveFavorite(code);

      languages.RemoveFavorite(language);

      WeakReferenceMessenger.Default.Send(new LanguageIsFavoriteChanged(code, false));
    }
  }
}