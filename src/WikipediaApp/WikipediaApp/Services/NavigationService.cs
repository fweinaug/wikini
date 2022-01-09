using System;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace WikipediaApp
{
  public class NavigationService : INavigationService
  {
    private Frame Frame
    {
      get
      {
        var shell = App.Current.AppShell;

        return shell.AppFrame;
      }
    }

    public void ShowArticle(ArticleHead article)
    {
      if (ShowArticleInCurrentArticlePage(article))
        return;

      var viewModel = new ArticlePageViewModel(article, App.Services.GetService<IWikipediaService>(), App.Services.GetService<IDialogService>(), App.Services.GetService<INavigationService>(), App.Services.GetService<IUserSettings>(), App.Services.GetService<IArticleViewModelFactory>());

      Frame.Navigate(typeof(ArticlePage), viewModel);
    }

    private bool ShowArticleInCurrentArticlePage(ArticleHead article)
    {
      var currentPage = Frame.Content as ArticlePage;

      var currentViewModel = currentPage?.DataContext as ArticlePageViewModel;
      if (currentViewModel == null)
        return false;

      currentViewModel.ShowArticle(article);
      return true;
    }

    public void ShowMap(string language)
    {
      var viewModel = App.Services.GetService<MapPageViewModel>();
      viewModel.Language = language;

      Frame.Navigate(typeof(MapPage), viewModel);
    }

    public async void OpenInBrowser(Uri uri)
    {
      await Launcher.LaunchUriAsync(uri);
    }
  }
}