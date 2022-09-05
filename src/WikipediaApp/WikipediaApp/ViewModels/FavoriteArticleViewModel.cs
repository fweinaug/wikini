using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;

namespace WikipediaApp
{
  public class FavoriteArticleViewModel
  {
    private RelayCommand removeFromFavoritesCommand;

    public FavoriteArticle Article { get; }

    public string Title => Article.Title;
    public string Language => Article.Language;
    public string Description => Article.Description;
    public bool HasDescription => Article.HasDescription;

    public ICommand RemoveFromFavoritesCommand
    {
      get { return removeFromFavoritesCommand ??= new RelayCommand(RemoveFromFavorites); }
    }

    public FavoriteArticleViewModel(FavoriteArticle article)
    {
      Article = article;
    }

    private void RemoveFromFavorites()
    {
      WeakReferenceMessenger.Default.Send(new RemoveArticleFromFavorites(Article));
    }
  }
}