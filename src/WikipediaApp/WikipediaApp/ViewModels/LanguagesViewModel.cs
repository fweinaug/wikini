using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;

namespace WikipediaApp
{
  #region Messages

  public sealed class AddLanguageToFavorites
  {
    public string Code { get; }

    public AddLanguageToFavorites(string code)
    {
      Code = code;
    }
  }

  public sealed class RemoveLanguageFromFavorites
  {
    public string Code { get; }

    public RemoveLanguageFromFavorites(string code)
    {
      Code = code;
    }
  }

  public sealed class LanguageIsFavoriteChanged
  {
    public string Code { get; private set; }
    public bool IsFavorite { get; private set; }

    public LanguageIsFavoriteChanged(string code, bool isFavorite)
    {
      Code = code;
      IsFavorite = isFavorite;
    }
  }

  #endregion

  public class LanguagesViewModel
  {
    private readonly IArticleLanguagesRepository articleLanguagesRepository;

    private readonly LanguageCollection languages = new LanguageCollection();

    public LanguageCollection All
    {
      get { return languages; }
    }

    public LanguageCollection.Group Favorites
    {
      get { return languages.Favorites; }
    }

    public LanguagesViewModel(IArticleLanguagesRepository articleLanguagesRepository)
    {
      this.articleLanguagesRepository = articleLanguagesRepository;

      WeakReferenceMessenger.Default.Register<LanguagesViewModel, AddLanguageToFavorites>(this, (_, message) =>
      {
        AddFavorite(message.Code);
      });
      WeakReferenceMessenger.Default.Register<LanguagesViewModel, RemoveLanguageFromFavorites>(this, (_, message) =>
      {
        RemoveFavorite(message.Code);
      });
    }

    public void UpdateLanguages(IEnumerable<Language> languages, IEnumerable<FavoriteLanguage> favorites)
    {
      var viewModels = languages.Select((language, index) => new LanguageViewModel(language, index)
      {
        IsFavorite = favorites.Any(favorite => favorite.Code == language.Code)
      });

      this.languages.UpdateLanguages(viewModels);
    }

    public LanguageViewModel GetLanguage(string code)
    {
      return languages.GetLanguage(code);
    }

    private void AddFavorite(string code)
    {
      var language = languages.GetLanguage(code);
      if (language == null)
        return;

      articleLanguagesRepository.AddFavorite(code);

      languages.AddFavorite(language);

      WeakReferenceMessenger.Default.Send(new LanguageIsFavoriteChanged(code, true));
    }

    public void RemoveFavorite(string code)
    {
      var language = languages.GetLanguage(code);
      if (language == null)
        return;

      articleLanguagesRepository.RemoveFavorite(code);

      languages.RemoveFavorite(language);

      WeakReferenceMessenger.Default.Send(new LanguageIsFavoriteChanged(code, false));
    }
  }
}