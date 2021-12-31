using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace WikipediaApp
{
  public sealed partial class AppShell : Page
  {
    private readonly AppShellViewModel viewModel = new AppShellViewModel();

    private ArticleHead initialArticle = null;

    public Frame AppFrame
    {
      get { return Frame; }
    }

    public AppShellViewModel ViewModel
    {
      get { return viewModel; }
    }

    public AppShell()
    {
      InitializeComponent();

      SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;

      viewModel = new AppShellViewModel();

      DataContext = viewModel;
    }

    private void SystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
    {
      if (Frame.CanGoBack)
      {
        Frame.GoBack();

        e.Handled = true;
      }
    }

    private void FrameNavigating(object sender, NavigatingCancelEventArgs e)
    {
      if (e.Parameter is ArticlePageViewModel viewModel)
      {
        AppIcon.Visibility = Visibility.Visible;
        AppTitle.ClearValue(TextBlock.TextProperty);
        AppTitle.SetBinding(TextBlock.TextProperty, new Binding { Source = viewModel, Path = new PropertyPath("Title") });
      }
      else
      {
        AppIcon.Visibility = Visibility.Collapsed;
        AppTitle.ClearValue(TextBlock.TextProperty);
        AppTitle.Text = Package.Current.DisplayName;
      }
    }

    private async void FrameNavigated(object sender, NavigationEventArgs e)
    {
      SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack
        ? AppViewBackButtonVisibility.Visible
        : AppViewBackButtonVisibility.Collapsed;

      if (e.Parameter is ViewModelBase frameViewModel)
        await frameViewModel.Initialize();
    }

    private async void PageLoaded(object sender, RoutedEventArgs e)
    {
      await viewModel.Initialize();

      if (initialArticle != null)
        viewModel.ShowArticle(initialArticle);
    }

    public async Task ShowLoadingError()
    {
      await ArticleLoadingFailedContentDialog.ShowAsync();
    }

    public void ShowArticle(ArticleHead article)
    {
      if (Frame.Content != null)
      {
        viewModel.ShowArticle(article);
      }
      else if (initialArticle == null)
      {
        initialArticle = article;
      }
    }
  }
}