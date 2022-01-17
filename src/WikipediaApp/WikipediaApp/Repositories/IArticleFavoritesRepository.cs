using System.Collections.Generic;
using System.Threading.Tasks;

namespace WikipediaApp
{
  public interface IArticleFavoritesRepository
  {
    Task<List<FavoriteArticle>> GetFavorites();

    FavoriteArticle AddArticle(ArticleHead article);
    void RemoveArticle(ArticleHead article);
  }
}