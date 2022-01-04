using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WikipediaApp
{
  public class LanguageGroup : Group<string, LanguageViewModel>
  {
  }

  public class EmptyFavoritesHint : LanguageViewModel, IDisabledListViewItem
  {
  }

  public class LanguageCollection : ObservableCollection<LanguageGroup>
  {
    private readonly LanguageGroup favoritesGroup = new LanguageGroup { Key = "LanguageGroupFavorites" };
    private readonly LanguageGroup otherGroup = new LanguageGroup { Key = "LanguageGroupMore" };

    private static readonly EmptyFavoritesHint EmptyFavoritesHint = new EmptyFavoritesHint();

    public LanguageGroup Favorites => favoritesGroup;

    public LanguageCollection()
    {
      Add(favoritesGroup);
      Add(otherGroup);
    }

    public void UpdateLanguages(IEnumerable<LanguageViewModel> languages)
    {
      favoritesGroup.Clear();
      otherGroup.Clear();

      if (languages != null)
      {
        foreach (var language in languages)
        {
          if (language.IsFavorite)
            favoritesGroup.Add(language);
          else
            otherGroup.Add(language);
        }
      }

      if (favoritesGroup.Count == 0)
        favoritesGroup.Add(EmptyFavoritesHint);
    }

    public LanguageViewModel GetLanguage(string code)
    {
      foreach (var language in favoritesGroup)
      {
        if (language == EmptyFavoritesHint)
          break;

        if (language.Code == code)
          return language;
      }

      foreach (var language in otherGroup)
      {
        if (language == EmptyFavoritesHint)
          break;

        if (language.Code == code)
          return language;
      }

      return null;
    }

    public void AddFavorite(LanguageViewModel language)
    {
      MoveLanguage(language, otherGroup, favoritesGroup);

      favoritesGroup.Remove(EmptyFavoritesHint);
    }

    public void RemoveFavorite(LanguageViewModel language)
    {
      MoveLanguage(language, favoritesGroup, otherGroup);

      if (favoritesGroup.Count == 0)
        favoritesGroup.Add(EmptyFavoritesHint);
    }

    private static void MoveLanguage(LanguageViewModel language, LanguageGroup from, LanguageGroup to)
    {
      var index = to.TakeWhile(x => language.Index > x.Index).Count();

      from.Remove(language);
      to.Insert(index, language);
    }
  }
}