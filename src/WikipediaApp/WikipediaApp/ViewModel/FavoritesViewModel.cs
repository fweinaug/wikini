using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace WikipediaApp
{
  #region Messages

  public sealed class AddArticleToFavorites : RequestMessage<bool>
  {
    public ArticleHead Article { get; private set; }

    public AddArticleToFavorites(ArticleHead article)
    {
      Article = article;
    }
  }
  
  public sealed class RemoveArticleFromFavorites : RequestMessage<bool>
  {
    public ArticleHead Article { get; private set; }

    public RemoveArticleFromFavorites(ArticleHead article)
    {
      Article = article;
    }
  }

  public sealed class IsArticleInFavorites : RequestMessage<bool>
  {
    public ArticleHead Article { get; private set; }

    public IsArticleInFavorites(ArticleHead article)
    {
      Article = article;
    }
  }

  public sealed class ArticleIsFavoriteChanged
  {
    public ArticleHead Article { get; private set; }
    public bool IsFavorite { get; private set; }

    public ArticleIsFavoriteChanged(ArticleHead article, bool isFavorite)
    {
      Article = article;
      IsFavorite = isFavorite;
    }
  }

  #endregion

  public class FavoritesViewModel : ViewModelBase
  {
    private readonly INavigationService navigationService;

    private RelayCommand<FavoriteArticleViewModel> showArticleCommand;

    public ObservableCollection<FavoriteArticleViewModel> All { get; } = new();

    public ICommand ShowArticleCommand
    {
      get { return showArticleCommand ??= new RelayCommand<FavoriteArticleViewModel>(ShowArticle); }
    }

    public FavoritesViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;

      WeakReferenceMessenger.Default.Register<FavoritesViewModel, AddArticleToFavorites>(this, (_, message) =>
      {
        AddArticle(message.Article);

        message.Reply(true);
      });
      WeakReferenceMessenger.Default.Register<FavoritesViewModel, RemoveArticleFromFavorites>(this, (_, message) =>
      {
        RemoveArticle(message.Article);

        message.Reply(false);
      });
      WeakReferenceMessenger.Default.Register<FavoritesViewModel, IsArticleInFavorites>(this, (_, message) =>
      {
        bool isFavorite = IsFavorite(message.Article);

        message.Reply(isFavorite);
      });
    }

    public override async Task Initialize()
    {
      var favorites = await ArticleFavorites.GetFavorites();

      favorites.ForEach(article =>
      {
        All.Add(new FavoriteArticleViewModel(article));
      });
    }

    private void AddArticle(ArticleHead article)
    {
      var favorite = ArticleFavorites.AddArticle(article);

      All.Insert(GetIndexByTitle(favorite), new FavoriteArticleViewModel(favorite));

      WeakReferenceMessenger.Default.Send(new ArticleIsFavoriteChanged(article, true));
    }

    private void RemoveArticle(ArticleHead article)
    {
      ArticleFavorites.RemoveArticle(article);

      if (All.FirstOrDefault(x => x.Language == article.Language && x.Article.PageId == article.PageId) is FavoriteArticleViewModel favorite)
      {
        All.Remove(favorite);

        WeakReferenceMessenger.Default.Send(new ArticleIsFavoriteChanged(article, false));
      }
    }

    private bool IsFavorite(ArticleHead article)
    {
      return All.Any(x => x.Language == article.Language && x.Article.PageId == article.PageId);
    }

    private void ShowArticle(FavoriteArticleViewModel article)
    {
      navigationService.ShowArticle(article.Article);
    }

    private int GetIndexByTitle(ArticleHead favorite)
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
  }
}