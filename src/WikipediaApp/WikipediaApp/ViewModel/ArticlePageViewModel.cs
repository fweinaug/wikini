using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace WikipediaApp
{
  public class ArticlePageViewModel : ViewModelBase
  {
    private readonly WikipediaService wikipediaService = new WikipediaService();
    private readonly DialogService dialogService = new DialogService();
    private readonly NavigationService navigationService = new NavigationService();

    private readonly ArticleHead initialArticle;

    private ArticleViewModel article;
    private ArticleFlyoutViewModel articleFlyout;
    private ArticleImageGalleryViewModel imageGallery;
    private bool isBusy = false;

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
          ImageGallery = new ArticleImageGalleryViewModel(article.Article);
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

    public IList<ArticleGroup> History
    {
      get { return ArticleHistory.All; }
    }

    public IList<ArticleHead> Favorites
    {
      get { return ArticleFavorites.All; }
    }

    public ArticlePageViewModel(ArticleHead initialArticle)
    {
      this.initialArticle = initialArticle;
      this.article = new ArticleViewModel(initialArticle);
    }

    public override async Task Initialize()
    {
      IsBusy = true;

      var article = await GetArticle(initialArticle);

      if (article != null)
      {
        Article = new ArticleViewModel(article);
        Article.AddArticleToHistory();
      }
      else
      {
        IsBusy = false;

        dialogService.ShowLoadingError();
      }
    }

    public async void ShowArticle(ArticleHead articleHead)
    {
      if (article.IsSameArticle(articleHead))
        return;

      IsBusy = true;

      if (articleHead is Article reopenedArticle)
      {
        Article = new ArticleViewModel(reopenedArticle);
      }
      else if (articleHead != null)
      {
        var article = await GetArticle(articleHead);

        Article = new ArticleViewModel(article);
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
        Article = new ArticleViewModel(updated);
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

      if (wikipediaService.IsWikipediaUri(uri))
      {
        var article = await GetArticle(uri);

        if (article != null)
        {
          Article = new ArticleViewModel(article);
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
      return await wikipediaService.GetArticle(uri, Settings.Current.ImagesDisabled);
    }

    private async Task<Article> GetArticle(ArticleHead articleHead)
    {
      return await wikipediaService.GetArticle(articleHead, Settings.Current.ImagesDisabled);
    }

    private async Task<Article> RefreshArticle(ArticleViewModel article)
    {
      return await wikipediaService.RefreshArticle(article.Article, Settings.Current.ImagesDisabled);
    }
  }
}