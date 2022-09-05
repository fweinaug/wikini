﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WikipediaApp
{
  public class ArticlePageViewModel : ObservableObject
  {
    private readonly IWikipediaService wikipediaService;
    private readonly IDialogService dialogService;
    private readonly INavigationService navigationService;
    private readonly IDisplay display;
    private readonly IAppSettings appSettings;
    private readonly IUserSettings userSettings;
    private readonly IArticleViewModelFactory articleViewModelFactory;

    private readonly ArticleHead initialArticle;

    private ArticleViewModel article;
    private ArticleFlyoutViewModel articleFlyout;
    private ArticleImageGalleryViewModel imageGallery;
    private bool isBusy = false;

    private AsyncRelayCommand loadCommand;
    private RelayCommand unloadCommand;
    private RelayCommand<ArticleLanguageViewModel> changeLanguageCommand;
    private RelayCommand refreshCommand;
    private RelayCommand<Uri> navigateCommand;
    private RelayCommand<Uri> loadedCommand;
    private RelayCommand<ArticleHead> showArticleCommand;

    public string Title
    {
      get { return article.Title; }
    }

    public ArticleViewModel Article
    {
      get { return article; }
      set {
        if (SetProperty(ref article, value))
        {
          OnPropertyChanged(nameof(Title));
          ImageGallery = new ArticleImageGalleryViewModel(article.Article, wikipediaService);

          appSettings.WriteLastArticle(article.Article);
        };
      }
    }

    public ArticleFlyoutViewModel ArticleFlyout
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

    public ArticleImageGalleryViewModel ImageGallery
    {
      get { return imageGallery; }
      private set { SetProperty(ref imageGallery, value); }
    }

    public bool IsBusy
    {
      get { return isBusy; }
      private set { SetProperty(ref isBusy, value); }
    }

    public bool SidebarInline
    {
      get { return userSettings.Get<bool>(UserSettingsKey.SplitViewInline); }
      set { userSettings.Set(UserSettingsKey.SplitViewInline, value); }
    }

    public AsyncRelayCommand LoadCommand
    {
      get { return loadCommand ?? (loadCommand = new AsyncRelayCommand(Initialize)); }
    }

    public RelayCommand UnloadCommand
    {
      get { return unloadCommand ?? (unloadCommand = new RelayCommand(Dispose)); }
    }

    public ICommand ChangeLanguageCommand
    {
      get { return changeLanguageCommand ?? (changeLanguageCommand = new RelayCommand<ArticleLanguageViewModel>(ChangeLanguage)); }
    }

    public ICommand RefreshCommand
    {
      get { return refreshCommand ?? (refreshCommand = new RelayCommand(Refresh)); }
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

    public ArticlePageViewModel(ArticleHead initialArticle, IWikipediaService wikipediaService, IDialogService dialogService, INavigationService navigationService, IDisplay display, IAppSettings appSettings, IUserSettings userSettings, IArticleViewModelFactory articleViewModelFactory)
    {
      this.initialArticle = initialArticle;
      this.article = articleViewModelFactory.GetArticle(initialArticle);

      this.wikipediaService = wikipediaService;
      this.dialogService = dialogService;
      this.navigationService = navigationService;
      this.display = display;
      this.appSettings = appSettings;
      this.userSettings = userSettings;
      this.articleViewModelFactory = articleViewModelFactory;
    }

    private async Task Initialize()
    {
      IsBusy = true;

      var article = await GetArticle(initialArticle);

      if (article != null)
      {
        Article = articleViewModelFactory.GetArticle(article);
        Article.AddArticleToHistory();
      }
      else
      {
        IsBusy = false;

        dialogService.ShowLoadingError();
      }

      userSettings.SettingSet += (sender, settingKey) =>
      {
        if (settingKey == UserSettingsKey.SplitViewInline)
          OnPropertyChanged(nameof(SidebarInline));
      };

      if (userSettings.Get<bool>(UserSettingsKey.DisplayActive))
        display.Activate();
    }

    private void Dispose()
    {
      display.Release();

      appSettings.WriteLastArticle(null);
    }

    public async void ShowArticle(ArticleHead articleHead)
    {
      if (article.IsSameArticle(articleHead))
        return;

      IsBusy = true;

      if (articleHead is Article reopenedArticle)
      {
        Article = articleViewModelFactory.GetArticle(reopenedArticle);
      }
      else if (articleHead != null)
      {
        var article = await GetArticle(articleHead);

        Article = articleViewModelFactory.GetArticle(article);
        Article.AddArticleToHistory();
      }
      else
      {
        IsBusy = false;

        dialogService.ShowLoadingError();
      }
    }

    private void ChangeLanguage(ArticleLanguageViewModel language)
    {
      Navigate(language.Uri);
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
        Article = articleViewModelFactory.GetArticle(updated);
      }
      else
      {
        IsBusy = false;

        dialogService.ShowLoadingError();
      }
    }

    private async void Navigate(Uri uri)
    {
      var isImage = await ImageGallery.NavigateToImage(uri);
      if (isImage)
        return;

      IsBusy = true;

      if (WikipediaUriParser.IsWikipediaUri(uri))
      {
        var article = await GetArticle(uri);

        if (article != null)
        {
          Article = articleViewModelFactory.GetArticle(article);
          Article.AddArticleToHistory();
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

    private async Task<Article> GetArticle(Uri uri)
    {
      return await wikipediaService.GetArticle(uri, userSettings.Get<bool>(UserSettingsKey.ImagesDisabled));
    }

    private async Task<Article> GetArticle(ArticleHead articleHead)
    {
      return await wikipediaService.GetArticle(articleHead, userSettings.Get<bool>(UserSettingsKey.ImagesDisabled));
    }

    private async Task<Article> RefreshArticle(ArticleViewModel article)
    {
      return await wikipediaService.RefreshArticle(article.Article, userSettings.Get<bool>(UserSettingsKey.ImagesDisabled));
    }
  }
}