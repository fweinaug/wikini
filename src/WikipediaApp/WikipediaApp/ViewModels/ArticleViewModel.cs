﻿using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace WikipediaApp
{
  public class ArticleViewModel : ObservableObject
  {
    private readonly IWikipediaService wikipediaService;
    private readonly INavigationService navigationService;
    private readonly IUserSettings userSettings;
    private readonly IShareManager shareManager;

    private readonly ArticleHead initialArticle;
    private readonly Article article;

    private IList<ArticleSectionViewModel> sections = null;
    private bool hasSections = false;
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

    public IList<ArticleSectionViewModel> Sections
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

    public ArticleLanguagesViewModel Languages { get; }

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

    public ArticleViewModel(ArticleHead initialArticle, IWikipediaService wikipediaService, INavigationService navigationService, IUserSettings userSettings, IShareManager shareManager)
      : this(wikipediaService, navigationService, userSettings, shareManager)
    {
      this.initialArticle = initialArticle;
    }

    public ArticleViewModel(Article article, IWikipediaService wikipediaService, INavigationService navigationService, IUserSettings userSettings, IShareManager shareManager, ArticleLanguagesViewModel articleLanguagesViewModel)
      : this(wikipediaService, navigationService, userSettings, shareManager)
    {
      this.article = article;

      Languages = articleLanguagesViewModel;
      HasLanguages = article.Languages.Count > 0;

      Sections = (userSettings.Get<bool>(UserSettingsKey.SectionsCollapsed) ? article.GetRootSections() : article.Sections).ConvertAll(section => new ArticleSectionViewModel(section));
      HasImages = article.Images?.Count > 0;
      IsFavorite = WeakReferenceMessenger.Default.Send(new IsArticleInFavorites(article));
    }

    private ArticleViewModel(IWikipediaService wikipediaService, INavigationService navigationService, IUserSettings userSettings, IShareManager shareManager)
    {
      this.wikipediaService = wikipediaService;
      this.navigationService = navigationService;
      this.userSettings = userSettings;
      this.shareManager = shareManager;

      WeakReferenceMessenger.Default.Register<ArticleViewModel, ArticleIsFavoriteChanged>(this, (_, message) =>
      {
        if (message.Article == (initialArticle ?? article))
        {
          IsFavorite = message.IsFavorite;
        }
      });
    }

    public bool IsSameArticle(ArticleHead articleHead)
    {
      var article = this.article ?? this.initialArticle;

      return (article.Language == articleHead.Language && article.PageId == articleHead.PageId)
        || (article.Uri != null && article.Uri == articleHead.Uri);
    }

    public async void AddArticleToHistory()
    {
      WeakReferenceMessenger.Default.Send(new AddArticleToHistory(article));

      if (userSettings.Get<bool>(UserSettingsKey.HistoryTimeline))
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
        IsFavorite = WeakReferenceMessenger.Default.Send(new AddArticleToFavorites(article ?? initialArticle));
      }
    }

    private void RemoveFromFavorites()
    {
      if (article != null)
      {
        IsFavorite = WeakReferenceMessenger.Default.Send(new RemoveArticleFromFavorites(article ?? initialArticle));
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
        shareManager.ShareArticle(shareArticle.Title, shareArticle.Uri);
      }
    }
  }
}