using System;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public class NavigationService
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

      var viewModel = new ArticlePageViewModel(article);

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
      var viewModel = new MapPageViewModel(language);

      Frame.Navigate(typeof(MapPage), viewModel);
    }

    public async void OpenInBrowser(Uri uri)
    {
      await Launcher.LaunchUriAsync(uri);
    }
  }
}