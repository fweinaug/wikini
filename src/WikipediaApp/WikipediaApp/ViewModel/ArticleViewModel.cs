using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace WikipediaApp
{
  public class ArticleViewModel : ViewModelBase
  {
    private readonly WikipediaService wikipediaService = new WikipediaService();
    private readonly NavigationService navigationService = new NavigationService();
    private readonly DialogService dialogService = new DialogService();

    private bool isBusy = false;

    private readonly ArticleHead initialArticle;

    private Article article = null;
    private IList<ArticleSection> sections = null;
    private bool hasSections = false;
    private IList<ArticleLanguage> languages = null;
    private bool hasLanguages = false;
    private bool isFavorite = false;
    private ArticleFlyout articleFlyout;

    private RelayCommand refreshCommand;
    private RelayCommand<ArticleLanguage> changeLanguageCommand;
    private RelayCommand openInBrowserCommand;
    private RelayCommand pinCommand;
    private RelayCommand addToFavoritesCommand;
    private RelayCommand removeFromFavoritesCommand;
    private RelayCommand shareCommand;

    private RelayCommand<Uri> navigateCommand;
    private RelayCommand<Uri> loadedCommand;
    private RelayCommand<ArticleHead> showArticleCommand;

    public string Title
    {
      get { return (article ?? initialArticle).Title; }
    }

    public bool IsBusy
    {
      get { return isBusy; }
      set { SetProperty(ref isBusy, value); }
    }

    public IList<ArticleGroup> History
    {
      get { return ArticleHistory.All; }
    }

    public IList<ArticleHead> Favorites
    {
      get { return ArticleFavorites.All; }
    }

    private ArticleImageGalleryViewModel imageGallery = null;

    public ArticleImageGalleryViewModel ImageGallery
    {
      get { return imageGallery; }
      private set { SetProperty(ref imageGallery, value); }
    }

    public Article Article
    {
      get { return article; }
      private set
      {
        if (SetProperty(ref article, value))
        {
          if (article != null)
          {
            Languages = article.Languages;
            Sections = Settings.Current.SectionsCollapsed ? article.GetRootSections() : article.Sections;
            IsFavorite = ArticleFavorites.IsFavorite(article);
            ImageGallery = new ArticleImageGalleryViewModel(article);

            AddArticleToHistory();

            OnPropertyChanged(nameof(Title));
          }
          else
          {
            Languages = null;
            Sections = null;
            IsFavorite = false;
            ImageGallery = null;
          }
        }
      }
    }

    public IList<ArticleSection> Sections
    {
      get { return sections; }
      private set
      {
        if (SetProperty(ref sections, value))
          HasSections = sections?.Count > 0;
      }
    }

    public bool HasSections
    {
      get { return hasSections; }
      private set { SetProperty(ref hasSections, value); }
    }

    public IList<ArticleLanguage> Languages
    {
      get { return languages; }
      private set
      {
        if (SetProperty(ref languages, value))
          HasLanguages = languages?.Count > 0;
      }
    }

    public bool HasLanguages
    {
      get { return hasLanguages; }
      private set { SetProperty(ref hasLanguages, value); }
    }

    public bool IsFavorite
    {
      get { return isFavorite; }
      private set { SetProperty(ref isFavorite, value); }
    }

    public ArticleFlyout ArticleFlyout
    {
      get { return articleFlyout; }
      set
      {
        if (SetProperty(ref articleFlyout, value)
            && articleFlyout != null && !articleFlyout.Loaded)
        {
          LoadArticleFlyout();
        }
      }
    }

    private async void LoadArticleFlyout()
    {
      var article = await wikipediaService.GetArticleInfo(articleFlyout.Uri);

      if (article != null)
        articleFlyout.Title = article.Title;
      else if (string.IsNullOrWhiteSpace(articleFlyout.Title))
        articleFlyout.Title = articleFlyout.Uri.ToString();

      articleFlyout.Article = article;
      articleFlyout.Loaded = true;
    }

    public ICommand RefreshCommand
    {
      get { return refreshCommand ?? (refreshCommand = new RelayCommand(Refresh)); }
    }

    public ICommand ChangeLanguageCommand
    {
      get { return changeLanguageCommand ?? (changeLanguageCommand = new RelayCommand<ArticleLanguage>(ChangeLanguage)); }
    }

    public ICommand OpenInBrowserCommand
    {
      get { return openInBrowserCommand ?? (openInBrowserCommand = new RelayCommand(OpenInBrowser)); }
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

    public ICommand NavigateCommand
    {
      get { return navigateCommand ?? (navigateCommand = new RelayCommand<Uri>(Navigate)); }
    }

    public ICommand LoadedCommand
    {
      get { return loadedCommand ?? (loadedCommand = new RelayCommand<Uri>(Loaded)); }
    }

    public ICommand ShowArticleCommand
    {
      get { return showArticleCommand ?? (showArticleCommand = new RelayCommand<ArticleHead>(ShowArticle)); }
    }

    public ArticleViewModel(ArticleHead initialArticle)
    {
      this.initialArticle = initialArticle;
    }

    private async void Refresh()
    {
      if (article == null && initialArticle == null)
        return;

      IsBusy = true;

      var updated = article != null
        ? await RefreshArticle(article)
        : await GetArticle(initialArticle);

      if (updated != null)
      {
        Article = null;
        Article = updated;
      }
      else
      {
        IsBusy = false;

        dialogService.ShowLoadingError();
      }
    }

    private async void AddArticleToHistory()
    {
      ArticleHistory.AddArticle(article);

      if (Settings.Current.HistoryTimeline)
      {
        await wikipediaService.AddArticleToTimeline(article);
      }
    }

    private void ChangeLanguage(ArticleLanguage language)
    {
      Navigate(language.Uri);
    }

    private async void OpenInBrowser()
    {
      var openArticle = article ?? initialArticle;
      if (openArticle == null)
        return;

      if (openArticle.Uri == null)
      {
        openArticle.Uri = await wikipediaService.GetArticleUri(openArticle.Language, openArticle.PageId, openArticle.Title);
      }

      if (openArticle.Uri != null)
      {
        navigationService.OpenInBrowser(openArticle.Uri);
      }
    }

    private async void Pin()
    {
      if (article != null)
      {
        await wikipediaService.PinArticle(article);
      }
      else if (initialArticle != null)
      {
        await wikipediaService.PinArticle(initialArticle.Language, initialArticle.PageId, initialArticle.Title);
      }
    }

    private void AddToFavorites()
    {
      if (article != null)
      {
        ArticleFavorites.AddArticle(article);

        IsFavorite = true;
      }
    }

    private void RemoveFromFavorites()
    {
      if (article != null)
      {
        ArticleFavorites.RemoveArticle(article);

        IsFavorite = false;
      }
    }

    private async void Share()
    {
      var shareArticle = article ?? initialArticle;
      if (shareArticle == null)
        return;

      if (shareArticle.Uri == null)
      {
        shareArticle.Uri = await wikipediaService.GetArticleUri(shareArticle.Language, shareArticle.PageId, shareArticle.Title);
      }

      if (shareArticle.Uri != null)
      {
        ShareManager.ShareArticle(shareArticle.Title, shareArticle.Uri);
      }
    }

    private async void Navigate(Uri uri)
    {
      var isImage = await ImageGallery.NavigateToImage(uri);
      if (isImage)
        return;

      IsBusy = true;

      if (wikipediaService.IsWikipediaUri(uri))
      {
        var article = await GetArticle(uri);

        if (article != null)
        {
          Article = article;
        }
        else
        {
          IsBusy = false;

          dialogService.ShowLoadingError();
        }
      }
      else
      {
        IsBusy = false;

        navigationService.OpenInBrowser(uri);
      }
    }

    private void Loaded(Uri uri)
    {
      IsBusy = false;
    }

    public async void ShowArticle(ArticleHead articleHead)
    {
      if (this.article != null && ((this.article.Language == articleHead.Language && this.article.PageId == articleHead.PageId)
                                   || (this.article.Uri != null && this.article.Uri == articleHead.Uri)))
        return;

      IsBusy = true;

      var article = articleHead as Article
                    ?? await GetArticle(articleHead);

      if (article != null)
      {
        Article = article;
      }
      else
      {
        IsBusy = false;

        dialogService.ShowLoadingError();
      }
    }

    public override async Task Initialize()
    {
      if (Article != null)
        return;

      IsBusy = true;

      var article = await GetArticle(initialArticle);

      if (article != null)
      {
        Article = article;
      }
      else
      {
        IsBusy = false;

        dialogService.ShowLoadingError();
      }
    }

    private async Task<Article> GetArticle(Uri uri)
    {
      return await wikipediaService.GetArticle(uri, Settings.Current.ImagesDisabled);
    }

    private async Task<Article> GetArticle(ArticleHead articleHead)
    {
      return await wikipediaService.GetArticle(articleHead, Settings.Current.ImagesDisabled);
    }

    private async Task<Article> RefreshArticle(Article article)
    {
      return await wikipediaService.RefreshArticle(article, Settings.Current.ImagesDisabled);
    }
  }
}