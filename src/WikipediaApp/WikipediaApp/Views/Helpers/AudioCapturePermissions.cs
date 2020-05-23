using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Media.Capture;
using Windows.System;
using Windows.UI.Popups;

namespace WikipediaApp
{
  public static class AudioCapturePermissions
  {
    private const int NoCaptureDevicesHResult = -1072845856;

    public static async Task<bool> RequestMicrophonePermission()
    {
      try
      {
        var settings = new MediaCaptureInitializationSettings
        {
          StreamingCaptureMode = StreamingCaptureMode.Audio,
          MediaCategory = MediaCategory.Speech
        };
        
        var capture = new MediaCapture();

        await capture.InitializeAsync(settings);

        return true;
      }
      catch (TypeLoadException)
      {
        await ShowLoadErrorMessage();

        return false;
      }
      catch (UnauthorizedAccessException)
      {
        await ShowPermissionDeniedMessage();

        return false;
      }
      catch (Exception ex)
      {
        if (ex.HResult == NoCaptureDevicesHResult)
        {
          await ShowNoDeviceErrorMessage();

          return false;
        }

        throw;
      }
    }

    private static async Task ShowPermissionDeniedMessage()
    {
      var resourceLoader = ResourceLoader.GetForCurrentView();

      var messageDialog = new MessageDialog(resourceLoader.GetString("AppSearchSpeechMicrophonePermissionMessageDialog/Content"), 
        resourceLoader.GetString("PermissionMessageDialog/Title"));

      messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("PermissionMessageDialog/Settings"),
        async _ => { await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app")); }));

      messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("PermissionMessageDialog/Close")));

      await messageDialog.ShowAsync();
    }

    private static async Task ShowLoadErrorMessage()
    {
      var resourceLoader = ResourceLoader.GetForCurrentView();

      var messageDialog = new MessageDialog(resourceLoader.GetString("AppSearchSpeechLoadErrorMessageDialog/Content"));
      await messageDialog.ShowAsync();
    }

    private static async Task ShowNoDeviceErrorMessage()
    {
      var resourceLoader = ResourceLoader.GetForCurrentView();

      var messageDialog = new MessageDialog(resourceLoader.GetString("AppSearchSpeechNoDeviceErrorMessageDialog/Content"));
      await messageDialog.ShowAsync();
    }
  }
}