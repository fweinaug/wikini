using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikipediaApp
{
  public partial class ArticleViewModel : ViewModelBase
  {
    private readonly WikipediaService wikipediaService = new WikipediaService();
    private readonly NavigationService navigationService = new NavigationService();
    private readonly DialogService dialogService = new DialogService();

    private bool isBusy = false;

    private Command showSettingsCommand = null;

    private readonly ArticleHead initialArticle;

    private Article article = null;
    private IList<ArticleSection> sections = null;
    private bool hasSections = false;
    private IList<ArticleLanguage> languages = null;
    private bool hasLanguages = false;
    private bool isFavorite = false;

    private Command refreshCommand;
    private Command<ArticleLanguage> changeLanguageCommand;
    private Command openInBrowserCommand;
    private Command pinCommand;
    private Command addToFavoritesCommand;
    private Command removeFromFavoritesCommand;

    private Command<Uri> navigateCommand;
    private Command<Uri> loadedCommand;
    private Command<ArticleHead> showArticleCommand;

    public bool IsBusy
    {
      get { return isBusy; }
      set { SetProperty(ref isBusy, value); }
    }

    public ICommand ShowSettingsCommand
    {
      get { return showSettingsCommand ?? (showSettingsCommand = new Command(ShowSettings)); }
    }

    public IList<ArticleHead> History
    {
      get { return ArticleHistory.All; }
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

            AddArticleToHistory();
          }
          else
          {
            Languages = null;
            Sections = null;
            IsFavorite = false;
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

    public ICommand RefreshCommand
    {
      get { return refreshCommand ?? (refreshCommand = new Command(Refresh)); }
    }

    public ICommand ChangeLanguageCommand
    {
      get { return changeLanguageCommand ?? (changeLanguageCommand = new Command<ArticleLanguage>(ChangeLanguage)); }
    }

    public ICommand OpenInBrowserCommand
    {
      get { return openInBrowserCommand ?? (openInBrowserCommand = new Command(OpenInBrowser)); }
    }

    public ICommand PinCommand
    {
      get { return pinCommand ?? (pinCommand = new Command(Pin)); }
    }

    public ICommand AddToFavoritesCommand
    {
      get { return addToFavoritesCommand ?? (addToFavoritesCommand = new Command(AddToFavorites)); }
    }

    public ICommand RemoveFromFavoritesCommand
    {
      get { return removeFromFavoritesCommand ?? (removeFromFavoritesCommand = new Command(RemoveFromFavorites)); }
    }

    public ICommand NavigateCommand
    {
      get { return navigateCommand ?? (navigateCommand = new Command<Uri>(Navigate)); }
    }

    public ICommand LoadedCommand
    {
      get { return loadedCommand ?? (loadedCommand = new Command<Uri>(Loaded)); }
    }

    public ICommand ShowArticleCommand
    {
      get { return showArticleCommand ?? (showArticleCommand = new Command<ArticleHead>(ShowArticle)); }
    }

    public ArticleViewModel(ArticleHead initialArticle)
    {
      this.initialArticle = initialArticle;
    }

    private void ShowSettings()
    {
      navigationService.ShowSettings();
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
        await wikipediaService.AddArticleToTimeline(article.Language, article.PageId, article.Title, article.Uri);
      }
    }

    private void ChangeLanguage(ArticleLanguage language)
    {
      Navigate(language.Uri);
    }

    private void OpenInBrowser()
    {
      var uri = article?.Uri ?? initialArticle?.Uri;

      if (uri != null)
        navigationService.OpenInBrowser(uri);
    }

    private async void Pin()
    {
      if (article != null)
      {
        await wikipediaService.PinArticle(article.Language, article.PageId, article.Title, article.Uri);
      }
      else if (initialArticle != null)
      {
        await wikipediaService.PinArticle(initialArticle.Language, initialArticle.PageId, initialArticle.Title, initialArticle.Uri);
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

    private async void Navigate(Uri uri)
    {
      var image = await NavigateToImage(uri);
      if (image)
        return;

      IsBusy = true;

      if (wikipediaService.IsWikipediaUri(uri))
      {
        var article = await GetArticle(uri);

        if (article != null)
        {
          Article = article;
          Images = null;
          SelectedImage = null;
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
        Images = null;
        SelectedImage = null;
      }
      else
      {
        IsBusy = false;

        dialogService.ShowLoadingError();
      }
    }

    public override async void Initialize()
    {
      if (Article != null)
        return;

      IsBusy = true;

      var article = await GetArticle(initialArticle);

      if (article != null)
      {
        Article = article;
        Images = null;
        SelectedImage = null;
      }
      else
      {
        IsBusy = false;

        dialogService.ShowLoadingError();
      }
    }

    private async Task<Article> GetArticle(Uri uri)
    {
      var article = ArticleCache.GetArticle(uri);
      if (article != null)
        return article;

      article = await wikipediaService.GetArticle(uri, Settings.Current.ImagesDisabled);
      if (article != null)
        ArticleCache.AddArticle(article);

      return article;
    }

    private async Task<Article> GetArticle(ArticleHead articleHead)
    {
      var article = ArticleCache.GetArticle(articleHead.Uri, articleHead.Language, articleHead.PageId);
      if (article != null)
        return article;

      article = await wikipediaService.GetArticle(articleHead, Settings.Current.ImagesDisabled);
      if (article != null)
        ArticleCache.AddArticle(article);

      return article;
    }

    private async Task<Article> RefreshArticle(Article article)
    {
      return await wikipediaService.RefreshArticle(article, Settings.Current.ImagesDisabled);
    }
  }
}