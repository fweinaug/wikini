using System;
using System.Windows.Input;

namespace WikipediaApp
{
  public class FavoriteArticle : ArticleHead
  {
    public int Id { get; set; }
    public DateTime Date { get; set; }

    private ICommand removeFromFavoritesCommand = null;

    public ICommand RemoveFromFavoritesCommand
    {
      get { return removeFromFavoritesCommand ?? (removeFromFavoritesCommand = new Command(RemoveFromFavorites)); }
    }

    private void RemoveFromFavorites()
    {
      ArticleFavorites.RemoveArticle(this);
    }
  }
}