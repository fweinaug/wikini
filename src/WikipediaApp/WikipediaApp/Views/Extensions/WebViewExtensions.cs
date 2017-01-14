using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public static class WebViewExtensions
  {
    public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
      "Content", typeof(string), typeof(WebViewExtensions), new PropertyMetadata(string.Empty, OnContentPropertyChanged));

    public static string GetContent(DependencyObject obj)
    {
      return (string)obj.GetValue(ContentProperty);
    }

    public static void SetContent(DependencyObject obj, string value)
    {
      obj.SetValue(ContentProperty, value);
    }

    private static async void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var content = e.NewValue as string;
      if (string.IsNullOrEmpty(content))
        return;

      await WebView.ClearTemporaryWebDataAsync();

      var webView = (WebView)d;
      webView.NavigateToString(content);
    }

    public static readonly DependencyProperty NavigatingCommandProperty = DependencyProperty.RegisterAttached(
      "NavigatingCommand", typeof(ICommand), typeof(WebViewExtensions), new PropertyMetadata(null, OnNavigatingCommandPropertyChanged));

    public static ICommand GetNavigatingCommand(DependencyObject obj)
    {
      return (ICommand)obj.GetValue(NavigatingCommandProperty);
    }

    public static void SetNavigatingCommand(DependencyObject obj, ICommand value)
    {
      obj.SetValue(NavigatingCommandProperty, value);
    }

    private static void OnNavigatingCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var webView = (WebView)d;

      webView.NavigationStarting -= WebViewNavigationStarting;

      var command = e.NewValue as ICommand;

      if (command != null)
        webView.NavigationStarting += WebViewNavigationStarting;
    }

    private static void WebViewNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs e)
    {
      if (e.Uri == null)
        return;

      var command = GetNavigatingCommand(sender);

      if (command != null && command.CanExecute(e.Uri))
      {
        command.Execute(e.Uri);

        e.Cancel = true;
      }
    }

    public static readonly DependencyProperty NavigatedCommandProperty = DependencyProperty.RegisterAttached(
      "NavigatedCommand", typeof(ICommand), typeof(WebViewExtensions), new PropertyMetadata(null, OnNavigatedCommandPropertyChanged));

    public static ICommand GetNavigatedCommand(DependencyObject obj)
    {
      return (ICommand)obj.GetValue(NavigatedCommandProperty);
    }

    public static void SetNavigatedCommand(DependencyObject obj, ICommand value)
    {
      obj.SetValue(NavigatedCommandProperty, value);
    }

    private static void OnNavigatedCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var webView = (WebView)d;

      webView.NavigationCompleted -= WebViewNavigationCompleted;

      var command = e.NewValue as ICommand;

      if (command != null)
        webView.NavigationCompleted += WebViewNavigationCompleted;
    }

    private static void WebViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs e)
    {
      var command = GetNavigatedCommand(sender);

      if (command != null && command.CanExecute(e.Uri))
        command.Execute(e.Uri);
    }

    public static async void ScrollToTop(this WebView webView)
    {
      var js = @"
        var elems = document.getElementsByTagName('body');
        if (elems.length > 0)
          elems[0].scrollIntoView();";

      await webView.InvokeScriptAsync("eval", new[] { js });
    }

    public static async void ScrollToElement(this WebView webView, string elementId)
    {
      var js = $@"
        var elem = document.getElementById('{elementId}');
        if (elem != null)
          elem.scrollIntoView();";

      await webView.InvokeScriptAsync("eval", new[] { js });
    }
  }
}