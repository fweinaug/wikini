using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage.Pickers;

namespace WikipediaApp
{
  public static class DownloadHelper
  {
    public static async Task DownloadImage(ArticleImage image)
    {
      var picker = BuildFileSavePicker(image);
      var file = await picker.PickSaveFileAsync();

      var downloader = new BackgroundDownloader();
      var download = downloader.CreateDownload(image.ImageUri, file);

      await download.StartAsync();
    }

    private static FileSavePicker BuildFileSavePicker(ArticleImage image)
    {
      var picker = new FileSavePicker
      {
        SuggestedStartLocation = PickerLocationId.PicturesLibrary,
        SuggestedFileName = image.Name,
      };

      var extension = Path.GetExtension(image.Name);
      picker.FileTypeChoices.Add(extension.Trim('.').ToUpper(), new[] { extension });

      return picker;
    }
  }
}