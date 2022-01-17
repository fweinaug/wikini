using System.Collections.Generic;
using System.Threading.Tasks;

namespace WikipediaApp
{
  public interface IArticleLanguagesRepository
  {
    List<FavoriteLanguage> Favorites { get; }

    Task<List<Language>> GetAll();
    Task<List<FavoriteLanguage>> GetFavorites();

    void AddFavorite(string code);
    void RemoveFavorite(string code);
  }
}