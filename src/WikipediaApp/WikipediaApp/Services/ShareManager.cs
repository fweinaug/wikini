using System;
using Windows.ApplicationModel.DataTransfer;

namespace WikipediaApp
{
  public static class ShareManager
  {
    public static void CopyToClipboard(Uri uri)
    {
      var dataPackage = new DataPackage();
      dataPackage.SetText(uri.OriginalString);

      Clipboard.SetContent(dataPackage);
    }

    public static void ShareArticle(string title, Uri uri)
    {
      RegisterDataRequestedHandler(title, uri);

      DataTransferManager.ShowShareUI(GetOptions());
    }

    private static void RegisterDataRequestedHandler(string title, Uri uri)
    {
      void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
      {
        DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;

        args.Request.Data.Properties.Title = title;
        args.Request.Data.SetWebLink(uri);
      }

      DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
    }

    private static ShareUIOptions GetOptions()
    {
      var theme = App.Current.InDarkMode() ? ShareUITheme.Dark : ShareUITheme.Light;

      return new ShareUIOptions { Theme = theme };
    }
  }
}