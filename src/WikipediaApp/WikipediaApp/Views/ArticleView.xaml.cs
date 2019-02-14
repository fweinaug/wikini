using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.HockeyApp;
using Newtonsoft.Json;

namespace WikipediaApp
{
  public sealed partial class ArticleView : UserControl
  {
    public event EventHandler ArticleChanged;

    public static readonly DependencyProperty ArticleProperty = DependencyProperty.RegisterAttached(
      "Article", typeof(Article), typeof(ArticleView), new PropertyMetadata(null, OnArticlePropertyChanged));

    public static readonly DependencyProperty NavigateCommandProperty = DependencyProperty.RegisterAttached(
      "NavigateCommand", typeof(ICommand), typeof(ArticleView), new PropertyMetadata(null));

    public static readonly DependencyProperty LoadedCommandProperty = DependencyProperty.RegisterAttached(
      "LoadedCommand", typeof(ICommand), typeof(ArticleView), new PropertyMetadata(null));

    public static readonly DependencyProperty ShowArticleCommandProperty = DependencyProperty.RegisterAttached(
      "ShowArticleCommand", typeof(ICommand), typeof(ArticleView), new PropertyMetadata(null));

    public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.RegisterAttached(
      "CanGoBack", typeof(bool), typeof(ArticleView), new PropertyMetadata(false));

    public static readonly DependencyProperty CanGoForwardProperty = DependencyProperty.RegisterAttached(
      "CanGoForward", typeof(bool), typeof(ArticleView), new PropertyMetadata(false));

    public static readonly DependencyProperty SearchResultsProperty = DependencyProperty.RegisterAttached(
      "SearchResults", typeof(int), typeof(ArticleView), new PropertyMetadata(0));

    public Article Article
    {
      get { return (Article)GetValue(ArticleProperty); }
      set { SetValue(ArticleProperty, value); }
    }

    public ICommand NavigateCommand
    {
      get { return (ICommand)GetValue(NavigateCommandProperty); }
      set { SetValue(NavigateCommandProperty, value); }
    }

    public ICommand LoadedCommand
    {
      get { return (ICommand)GetValue(LoadedCommandProperty); }
      set { SetValue(LoadedCommandProperty, value); }
    }

    public ICommand ShowArticleCommand
    {
      get { return (ICommand)GetValue(ShowArticleCommandProperty); }
      set { SetValue(ShowArticleCommandProperty, value); }
    }

    public bool CanGoBack
    {
      get { return (bool)GetValue(CanGoBackProperty); }
      private set { SetValue(CanGoBackProperty, value); }
    }

    public bool CanGoForward
    {
      get { return (bool)GetValue(CanGoForwardProperty); }
      private set { SetValue(CanGoForwardProperty, value); }
    }

    public int SearchResults
    {
      get { return (int)GetValue(SearchResultsProperty); }
      private set { SetValue(SearchResultsProperty, value); }
    }

    private readonly List<ArticleStackEntry> backStack = new List<ArticleStackEntry>();
    private readonly List<ArticleStackEntry> forwardStack = new List<ArticleStackEntry>();

    public ArticleView()
    {
      InitializeComponent();
    }

    private static void OnArticlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var article = e.NewValue as Article;
      if (string.IsNullOrEmpty(article?.Content))
        return;

      var view = (ArticleView)d;
      view.ShowArticle(article, e.OldValue as Article);
    }

    private async void ShowArticle(Article article, Article currentArticle)
    {
      try
      {
        await WebView.ClearTemporaryWebDataAsync();
      }
      catch (Exception)
      {
      }

      SearchResults = 0;

      if (currentArticle != null && currentArticle.PageId != article.PageId)
      {
        await UpdateStacks(article, currentArticle);
      }

      var html = WikipediaHtmlBuilder.BuildArticle(article.Title, article.Content, article.Language);

      WebView.NavigateToString(html);

      ArticleChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task UpdateStacks(Article newArticle, Article currentArticle)
    {
      var position = await WebView.GetScrollPosition();

      var entry = currentArticle as ArticleStackEntry;
      if (entry == null)
      {
        entry = new ArticleStackEntry
        {
          Language = currentArticle.Language,
          PageId = currentArticle.PageId,
          Title = currentArticle.Title,
          Content = currentArticle.Content,
          Uri = currentArticle.Uri,
          Sections = currentArticle.Sections,
          Languages = currentArticle.Languages,
          Images = currentArticle.Images,
          Position = position
        };
      }
      else
      {
        entry.Position = position;
      }

      if (newArticle is ArticleStackEntry)
      {
        var gotoEntry = (ArticleStackEntry)newArticle;
        if (backStack.Contains(gotoEntry))
        {
          backStack.Remove(gotoEntry);
          forwardStack.Insert(0, entry);
        }
        else
        {
          backStack.Add(entry);
          forwardStack.Remove(gotoEntry);
        }
      }
      else
      {
        backStack.Add(entry);
        forwardStack.Clear();
      }

      CanGoBack = backStack.Count > 0;
      CanGoForward = forwardStack.Count > 0;
    }

    private void WebViewNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs e)
    {
      if (e.Uri == null)
        return;

      if (e.Uri.AbsolutePath == "/" && e.Uri.Fragment.StartsWith("#"))
      {
        e.Cancel = true;

        var id = e.Uri.Fragment.Substring(1);

        WebView.ScrollToElement(id);
      }
      else
      {
        var command = NavigateCommand;

        if (command != null && command.CanExecute(e.Uri))
        {
          e.Cancel = true;

          command.Execute(e.Uri);
        }
      }
    }

    private void WebViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs e)
    {
      if (e.IsSuccess)
      {
        if (!string.IsNullOrEmpty(Article?.Anchor))
          WebView.ScrollToElement(Article.Anchor);
      }

      var command = LoadedCommand;

      if (command != null && command.CanExecute(e.Uri))
      {
        command.Execute(e.Uri);
      }
    }

    private void WebViewScriptNotify(object sender, NotifyEventArgs e)
    {
      SearchResults = Convert.ToInt32(e.Value);
    }

    public void ScrollToTop()
    {
      WebView.ScrollToTop();
    }

    public void ScrollToSection(ArticleSection section)
    {
      var anchor = section.Anchor;

      WebView.ScrollToElement(anchor);
    }

    public void GoBack()
    {
      if (backStack.Count == 0)
        return;

      var entry = backStack[backStack.Count - 1];

      GoToStackEntry(entry);
    }

    public void GoForward()
    {
      if (forwardStack.Count == 0)
        return;

      var entry = forwardStack[0];

      GoToStackEntry(entry);
    }

    public async void Search(string query)
    {
      try
      {
        await WebView.InvokeScriptAsync("search", new[] { query });
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);
      }
    }

    public async void SearchForward()
    {
      try
      {
        await WebView.InvokeScriptAsync("searchForward", null);
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);
      }
    }

    public async void SearchBackward()
    {
      try
      {
        await WebView.InvokeScriptAsync("searchBackward", null);
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);
      }
    }

    private void GoToStackEntry(ArticleStackEntry entry)
    {
      var position = entry.Position;

      if (position > 0d)
      {
        TypedEventHandler<WebView, WebViewNavigationCompletedEventArgs> webViewOnNavigationCompleted = null;
        webViewOnNavigationCompleted = async (sender, e) =>
        {
          WebView.NavigationCompleted -= webViewOnNavigationCompleted;

          if (e.IsSuccess)
            await WebView.ScrollToPosition(position);
        };

        WebView.NavigationCompleted += webViewOnNavigationCompleted;
      }

      var command = ShowArticleCommand;

      if (command != null && command.CanExecute(entry))
        ShowArticleCommand.Execute(entry);
    }

    private class ArticleStackEntry : Article
    {
      public double Position { get; set; }
    }
  }
}