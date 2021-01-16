using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.AppCenter.Crashes;

namespace WikipediaApp
{
  public static class WebViewExtensions
  {
    public static async void ScrollToElement(this WebView webView, string elementId)
    {
      try
      {
        var js = $"scrollToElement('{elementId}');";

        await webView.InvokeScriptAsync("eval", new[] { js });
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);
      }
    }

    public static async Task ScrollToPosition(this WebView webView, double position)
    {
      try
      {
        var js = $"window.scrollTo(0,{position});";

        await webView.InvokeScriptAsync("eval", new[] { js });
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);
      }
    }

    public static async Task<double> GetScrollPosition(this WebView webView)
    {
      try
      {
        const string js = "window.pageYOffset.toString();";

        var y = await webView.InvokeScriptAsync("eval", new[] { js });

        double position;
        if (!string.IsNullOrEmpty(y) && double.TryParse(y, out position))
          return position;

        return 0d;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return 0d;
      }
    }
  }
}