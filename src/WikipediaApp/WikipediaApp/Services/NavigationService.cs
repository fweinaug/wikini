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
      var viewModel = new ArticleViewModel(article);

      Frame.Navigate(typeof(ArticlePage), viewModel);
    }

    public void ShowSettings()
    {
      Frame.Navigate(typeof(SettingsPage), null);
    }

    public async void OpenInBrowser(Uri uri)
    {
      await Launcher.LaunchUriAsync(uri);
    }
  }
}