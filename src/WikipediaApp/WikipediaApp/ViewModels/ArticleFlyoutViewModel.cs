using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace WikipediaApp
{
  public class ArticleFlyoutViewModel : ObservableObject
  {
    private readonly IWikipediaService wikipediaService;
    private readonly IShareManager shareManager;

    private bool loaded = false;

    private Uri uri;
    private double left;
    private double top;
    private string title;

    private ArticleHead article;
    private bool isArticle;
    private bool? isFavorite;

    private RelayCommand pinCommand;
    private RelayCommand addToFavoritesCommand;
    private RelayCommand removeFromFavoritesCommand;
    private RelayCommand shareCommand;
    private RelayCommand copyToClipboardCommand;

    public bool Loaded
    {
      get { return loaded; }
      set { SetProperty(ref loaded, value); }
    }

    public Uri Uri
    {
      get { return uri; }
      set { SetProperty(ref uri, value); }
    }

    public double Left
    {
      get { return left; }
      set { SetProperty(ref left, value); }
    }

    public double Top
    {
      get { return top; }
      set { SetProperty(ref top, value); }
    }

    public string Title
    {
      get { return title; }
      set { SetProperty(ref title, value); }
    }

    public ArticleHead Article
    {
      get { return article; }
      set
      {
        if (SetProperty(ref article, value))
        {
          if (value != null)
          {
            IsArticle = true;
            IsFavorite = WeakReferenceMessenger.Default.Send(new IsArticleInFavorites(article));
          }
          else
          {
            IsArticle = false;
            IsFavorite = null;
          }
        }
      }
    }

    public bool IsArticle
    {
      get { return isArticle; }
      private set { SetProperty(ref isArticle, value); }
    }

    public bool? IsFavorite
    {
      get { return isFavorite; }
      private set { SetProperty(ref isFavorite, value); }
    }

    public ICommand PinCommand
    {
      get { return pinCommand ?? (pinCommand = new RelayCommand(Pin)); }
    }

    public ICommand AddToFavoritesCommand
    {
      get { return addToFavoritesCommand ?? (addToFavoritesCommand = new RelayCommand(AddToFavorites)); }
    }

    public ICommand RemoveFromFavoritesCommand
    {
      get { return removeFromFavoritesCommand ?? (removeFromFavoritesCommand = new RelayCommand(RemoveFromFavorites)); }
    }

    public ICommand ShareCommand
    {
      get { return shareCommand ?? (shareCommand = new RelayCommand(Share)); }
    }

    public ICommand CopyToClipboardCommand
    {
      get { return copyToClipboardCommand ?? (copyToClipboardCommand = new RelayCommand(CopyToClipboard)); }
    }

    public ArticleFlyoutViewModel(IWikipediaService wikipediaService, IShareManager shareManager)
    {
      this.wikipediaService = wikipediaService;
      this.shareManager = shareManager;
    }

    private async void Pin()
    {
      if (article != null)
      {
        await wikipediaService.PinArticle(article);
      }
    }

    private void AddToFavorites()
    {
      if (article != null)
      {
        IsFavorite = WeakReferenceMessenger.Default.Send(new AddArticleToFavorites(article));
      }
    }

    private void RemoveFromFavorites()
    {
      if (article != null)
      {
        IsFavorite = WeakReferenceMessenger.Default.Send(new RemoveArticleFromFavorites(article));
      }
    }

    private void Share()
    {
      if (article != null)
      {
        shareManager.ShareArticle(Title, article.Uri);
      }
      else
      {
        shareManager.ShareArticle(Title, uri);
      }
    }

    private void CopyToClipboard()
    {
      if (article != null)
      {
        shareManager.CopyToClipboard(article.Uri);
      }
      else
      {
        shareManager.CopyToClipboard(uri);
      }
    }
  }
}