using Windows.UI.ViewManagement;

namespace WikipediaApp
{
  public class SystemSettingProvider : ISystemSettingProvider
  {
    private readonly UISettings uiSettings = new();

    public double TextScaleFactor => uiSettings.TextScaleFactor;
  }
}