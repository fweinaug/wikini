using Windows.System.Display;

namespace WikipediaApp
{
  public static class DisplayHelper
  {
    private static DisplayRequest displayRequest = null;

    public static void ActivateDisplay()
    {
      if (displayRequest == null)
        displayRequest = new DisplayRequest();

      displayRequest.RequestActive();
    }

    public static void ReleaseDisplay()
    {
      displayRequest?.RequestRelease();
    }
  }
}