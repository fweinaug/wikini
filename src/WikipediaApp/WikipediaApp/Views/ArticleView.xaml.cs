using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;

namespace WikipediaApp
{
  public sealed partial class ArticleView : UserControl
  {
    public event EventHandler ArticleChanged;

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
      nameof(Header), typeof(double), typeof(ArticleView), new PropertyMetadata(0.0, OnHeaderPropertyChanged));

    public static readonly DependencyProperty ArticleProperty = DependencyProperty.Register(
      nameof(Article), typeof(Article), typeof(ArticleView), new PropertyMetadata(null, OnArticlePropertyChanged));

    public static readonly DependencyProperty NavigateCommandProperty = DependencyProperty.Register(
      nameof(NavigateCommand), typeof(ICommand), typeof(ArticleView), new PropertyMetadata(null));

    public static readonly DependencyProperty LoadedCommandProperty = DependencyProperty.Register(
      nameof(LoadedCommand), typeof(ICommand), typeof(ArticleView), new PropertyMetadata(null));

    public static readonly DependencyProperty ShowArticleCommandProperty = DependencyProperty.Register(
      nameof(ShowArticleCommand), typeof(ICommand), typeof(ArticleView), new PropertyMetadata(null));

    public static readonly DependencyProperty ArticleFlyoutProperty = DependencyProperty.Register(
      nameof(ArticleFlyout), typeof(ArticleFlyoutViewModel), typeof(ArticleView), new PropertyMetadata(null));

    public static readonly DependencyProperty FlyoutProperty = DependencyProperty.Register(
      nameof(Flyout), typeof(FlyoutBase), typeof(ArticleView), new PropertyMetadata(null));

    public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(
      nameof(CanGoBack), typeof(bool), typeof(ArticleView), new PropertyMetadata(false));

    public static readonly DependencyProperty CanGoForwardProperty = DependencyProperty.Register(
      nameof(CanGoForward), typeof(bool), typeof(ArticleView), new PropertyMetadata(false));

    public static readonly DependencyProperty SearchResultsProperty = DependencyProperty.Register(
      nameof(SearchResults), typeof(int), typeof(ArticleView), new PropertyMetadata(0));

    public double Header
    {
      get { return (double)GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }
    
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

    public ArticleFlyoutViewModel ArticleFlyout
    {
      get { return (ArticleFlyoutViewModel)GetValue(ArticleFlyoutProperty); }
      set { SetValue(ArticleFlyoutProperty, value); }
    }

    public FlyoutBase Flyout
    {
      get { return (FlyoutBase)GetValue(FlyoutProperty); }
      set { SetValue(FlyoutProperty, value); }
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

    private async void OnActualThemeChanged(FrameworkElement sender, object args)
    {
      try
      {
        var theme = ActualTheme == ElementTheme.Light ? "theme-light" : "theme-dark";

        var js = $"if (typeof changeTheme !== 'undefined') changeTheme('{theme}');";

        await WebView.InvokeScriptAsync("eval", new[] { js });
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);
      }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      Settings.Current.PropertyChanged += OnSettingsPropertyChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      Settings.Current.PropertyChanged -= OnSettingsPropertyChanged;
    }

    private async void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      try
      {
        if (e.PropertyName == nameof(Settings.Typeface))
        {
          var js = WikipediaHtmlBuilder.BuildTypefaceUpdateJS();

          await WebView.InvokeScriptAsync("eval", new[] { js });
        }
        else if (e.PropertyName == nameof(Settings.FontSize))
        {
          var js = WikipediaHtmlBuilder.BuildFontSizeUpdateJS();

          await WebView.InvokeScriptAsync("eval", new[] { js });
        }
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);
      }
    }

    private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var header = (double)e.NewValue;

      var view = (ArticleView)d;
      view.UpdateHeader(header);
    }

    private static void OnArticlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var article = e.NewValue as Article;
      if (string.IsNullOrEmpty(article?.Content))
        return;

      var view = (ArticleView)d;
      view.ShowArticle(article, e.OldValue as Article);
    }

    private async void UpdateHeader(double header)
    {
      ScrollBar.Margin = new Thickness(0, header, 0, 0);

      try
      {
        var js = $"document.body.style.marginTop = '{header + 10}px';";

        await WebView.InvokeScriptAsync("eval", new[] { js });
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);
      }
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

      var html = WikipediaHtmlBuilder.BuildArticle(article.Title, article.Description, article.Content, article.Language, article.TextDirection, Convert.ToInt32(Header) + 10);

      WebView.NavigateToString(html);

      ArticleChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task UpdateStacks(Article newArticle, Article currentArticle)
    {
      var position = await WebView.GetScrollPosition();

      var entry = currentArticle as ArticleStackEntry;
      if (entry == null)
      {
        entry = new ArticleStackEntry(currentArticle)
        {
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
      var article = (ArticleFlyoutViewModel)item.DataContext;

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

      LoadedCommand?.Execute(e.Uri);
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
          if (data.HasUrl)
            ShowArticleFlyout(data);
          else if (data.HasText)
            ShowSelectionFlyout(data);
          else
            ShowFlyout(data);
          break;
        case ScriptNotifyData.Scroll:
          UpdateScrollPosition(data);
          break;
      }
    }

    private async void ScrollBarScroll(object sender, ScrollEventArgs e)
    {
      await WebView.ScrollToPosition(e.NewValue);
    }

    public async void ScrollToTop()
    {
      try
      {
        var js = "scrollToTop();";

        await WebView.InvokeScriptAsync("eval", new[] { js });
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);
      }
    }

    public async void ScrollToSection(string id)
    {
      try
      {
        var js = $"scrollToSection('{id}');";

        await WebView.InvokeScriptAsync("eval", new[] { js });
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);
      }
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
        Crashes.TrackError(ex);
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
        Crashes.TrackError(ex);
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
        Crashes.TrackError(ex);
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

      ShowArticleCommand?.Execute(entry);
    }

    private void ShowFlyout(ScriptNotifyData data)
    {
      Flyout.ShowAt(WebView, new FlyoutShowOptions { Position = new Point(data.X, data.Y) });
    }

    private void ShowSelectionFlyout(ScriptNotifyData data)
    {
      SelectionMenuFlyout.ShowAt(WebView, new FlyoutShowOptions { Position = new Point(data.X, data.Y) });
    }

    private async void CopySelectionClick(object sender, RoutedEventArgs e)
    {
      var selectedContent = await WebView.CaptureSelectedContentToDataPackageAsync();

      Clipboard.SetContent(selectedContent);
    }

    private void ShowArticleFlyout(ScriptNotifyData data)
    {
      var viewModel = new ArticleFlyoutViewModel
      {
        Uri = new Uri(data.Url),
        Left = data.X,
        Top = data.Y + data.Height + 5,
        Title = data.Text
      };

      void OnArticleFlyoutPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (e.PropertyName == nameof(ArticleFlyout.Loaded))
        {
          viewModel.PropertyChanged -= OnArticleFlyoutPropertyChanged;

          LinkMenuFlyout.ShowAt(WebView, new FlyoutShowOptions { Position = new Point(viewModel.Left, viewModel.Top) });
        }
      }

      viewModel.PropertyChanged += OnArticleFlyoutPropertyChanged;

      ArticleFlyout = viewModel;
    }

    private void UpdateScrollPosition(ScriptNotifyData data)
    {
      if (data.Height > 0)
      {
        ScrollBar.Maximum = data.Height;
        ScrollBar.ViewportSize = ActualHeight;
        ScrollBar.Value = data.Y;
        ScrollBar.SmallChange = ActualHeight * 0.25;
        ScrollBar.LargeChange = ActualHeight * 0.8;
        ScrollBar.Visibility = Visibility.Visible;
      }
      else
      {
        ScrollBar.Visibility = Visibility.Collapsed;
        ScrollBar.Value = 0;
        ScrollBar.ViewportSize = 0;
        ScrollBar.Maximum = 0;
      }

      foreach (var section in Article.Sections)
      {
        section.IsActive = section.Anchor == data.Text;
      }
    }

    private class ArticleStackEntry : Article
    {
      public double Position { get; set; }

      public ArticleStackEntry(Article article) : base(article)
      {
      }
    }

    private class ScriptNotifyData
    {
      public const string SearchResults = "SearchResults";
      public const string Contextmenu = "Contextmenu";
      public const string Scroll = "Scroll";

      public string Message { get; set; }
      public double X { get; set; }
      public double Y { get; set; }
      public double Width { get; set; }
      public double Height { get; set; }
      public string Url { get; set; }
      public string Text { get; set; }
      public int Number { get; set; }

      public bool HasUrl => !string.IsNullOrWhiteSpace(Url);
      public bool HasText => !string.IsNullOrWhiteSpace(Text);
    }
  }
}