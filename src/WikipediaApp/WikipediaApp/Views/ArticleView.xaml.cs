using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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

    public static readonly DependencyProperty ArticleFlyoutProperty = DependencyProperty.RegisterAttached(
      "ArticleFlyout", typeof(ArticleFlyout), typeof(ArticleView), new PropertyMetadata(null));

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

    public ArticleFlyout ArticleFlyout
    {
      get { return (ArticleFlyout)GetValue(ArticleFlyoutProperty); }
      set { SetValue(ArticleFlyoutProperty, value); }
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

    private void NavigateLinkMenuFlyoutItem(object sender, RoutedEventArgs e)
    {
      var item = (MenuFlyoutItem)sender;
      var article = (ArticleFlyout)item.DataContext;

      NavigateToUri(article.Uri);
    }

    private void WebViewNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs e)
    {
      if (e.Uri != null)
      {
        e.Cancel = NavigateToUri(e.Uri);
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
      var data = JsonConvert.DeserializeObject<ScriptNotifyData>(e.Value);

      switch (data.Message)
      {
        case ScriptNotifyData.SearchResults:
          SearchResults = Convert.ToInt32(data.Number);
          break;
        case ScriptNotifyData.Contextmenu:
          ShowArticleFlyout(data);
          break;
      }
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

    private bool NavigateToUri(Uri uri)
    {
      if (uri.AbsolutePath == "/" && uri.Fragment.StartsWith("#"))
      {
        var id = uri.Fragment.Substring(1);

        WebView.ScrollToElement(id);

        return true;
      }

      var command = NavigateCommand;
      if (command != null && command.CanExecute(uri))
      {
        command.Execute(uri);

        return true;
      }

      return false;
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

    private void ShowArticleFlyout(ScriptNotifyData data)
    {
      var flyout = new ArticleFlyout
      {
        Uri = new Uri(data.Url),
        Left = data.X,
        Top = data.Y + data.Height + 5,
        Title = data.Text
      };

      void OnArticleFlyoutOnPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (e.PropertyName == nameof(ArticleFlyout.Loaded))
        {
          flyout.PropertyChanged -= OnArticleFlyoutOnPropertyChanged;

          LinkMenuFlyout.ShowAt(WebView, new FlyoutShowOptions { Position = new Point(flyout.Left, flyout.Top) });
        }
      }

      flyout.PropertyChanged += OnArticleFlyoutOnPropertyChanged;

      ArticleFlyout = flyout;
    }

    private class ArticleStackEntry : Article
    {
      public double Position { get; set; }
    }

    private class ScriptNotifyData
    {
      public const string SearchResults = "SearchResults";
      public const string Contextmenu = "Contextmenu";

      public string Message { get; set; }
      public double X { get; set; }
      public double Y { get; set; }
      public double Width { get; set; }
      public double Height { get; set; }
      public string Url { get; set; }
      public string Text { get; set; }
      public int Number { get; set; }
    }
  }
}