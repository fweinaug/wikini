using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace WikipediaApp
{
  public class ArticleViewModel : ViewModelBase
  {
    private readonly WikipediaService wikipediaService = new WikipediaService();
    private readonly NavigationService navigationService = new NavigationService();

    private readonly ArticleHead initialArticle;
    private readonly Article article;

    private IList<ArticleSection> sections = null;
    private bool hasSections = false;
    private IList<ArticleLanguage> languages = null;
    private bool hasLanguages = false;
    private bool hasImages = false;
    private bool isFavorite = false;

    private RelayCommand openInBrowserCommand;
    private RelayCommand pinCommand;
    private RelayCommand addToFavoritesCommand;
    private RelayCommand removeFromFavoritesCommand;
    private RelayCommand shareCommand;

    public string Language => article?.Language;
    public string Title => (article ?? initialArticle).Title;
    public string Description => (article ?? initialArticle).Description;
    public string Content => article?.Content;
    public string TextDirection => article?.TextDirection;
    public string Anchor => article?.Anchor;

    public Article Article
    {
      get { return article; }
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

    public bool HasImages
    {
      get { return hasImages; }
      private set { SetProperty(ref hasImages, value); }
    }

    public bool IsFavorite
    {
      get { return isFavorite; }
      private set { SetProperty(ref isFavorite, value); }
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

    public ArticleViewModel(ArticleHead initialArticle)
    {
      this.initialArticle = initialArticle;
    }

    public ArticleViewModel(Article article)
    {
      this.article = article;

      Languages = article.Languages;
      Sections = Settings.Current.SectionsCollapsed ? article.GetRootSections() : article.Sections;
      HasImages = article.Images?.Count > 0;
      IsFavorite = ArticleFavorites.IsFavorite(article);
    }

    public bool IsSameArticle(ArticleHead articleHead)
    {
      var article = this.article ?? this.initialArticle;

      return (article.Language == articleHead.Language && article.PageId == articleHead.PageId)
        || (article.Uri != null && article.Uri == articleHead.Uri);
    }

    public async void AddArticleToHistory()
    {
      ArticleHistory.AddArticle(article);

      if (Settings.Current.HistoryTimeline)
      {
        await wikipediaService.AddArticleToTimeline(article);
      }
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
  }
}