using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Media.SpeechRecognition;
using Windows.System;
using Windows.UI.Popups;

namespace WikipediaApp
{
  public static class SpeechRecognitionHelper
  {
    private const uint HResultPrivacyStatementDeclined = 0x80045509;

    public static async Task<string> RecognizeSearchTerm()
    {
      var permission = await AudioCapturePermissions.RequestMicrophonePermission();
      if (!permission)
        return null;

      try
      {
        var resourceLoader = ResourceLoader.GetForCurrentView();

        var speechRecognizer = new SpeechRecognizer();
        speechRecognizer.UIOptions.AudiblePrompt = resourceLoader.GetString("AppSearchSpeechRecognitionUI/Prompt");
        speechRecognizer.UIOptions.ExampleText = resourceLoader.GetString("AppSearchSpeechRecognitionUI/Example");

        var webSearchGrammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.WebSearch, "webSearch");
        speechRecognizer.Constraints.Add(webSearchGrammar);

        await speechRecognizer.CompileConstraintsAsync();

        var speechRecognitionResult = await speechRecognizer.RecognizeWithUIAsync();

        return speechRecognitionResult.Status == SpeechRecognitionResultStatus.Success
          ? speechRecognitionResult.Text
          : null;
      }
      catch (Exception ex)
      {
        if ((uint)ex.HResult == HResultPrivacyStatementDeclined)
        {
          await ShowPrivacyStatementDeclinedMessage();
        }
        else
        {
          await ShowErrorMessage(ex);
        }

        return null;
      }
    }

    private static async Task ShowPrivacyStatementDeclinedMessage()
    {
      var resourceLoader = ResourceLoader.GetForCurrentView();

      var messageDialog = new MessageDialog(resourceLoader.GetString("AppSearchSpeechPrivacyPermissionMessageDialog/Content"), 
        resourceLoader.GetString("PermissionMessageDialog/Title"));

      messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("PermissionMessageDialog/Settings"),
        async _ => { await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-speech")); }));

      messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("PermissionMessageDialog/Close")));

      await messageDialog.ShowAsync();
    }

    private static async Task ShowErrorMessage(Exception exception)
    {
      var messageDialog = new MessageDialog(exception.Message);

      await messageDialog.ShowAsync();
    }
  }
}