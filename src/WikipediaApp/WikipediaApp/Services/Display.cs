using Windows.System.Display;

namespace WikipediaApp
{
  public class Display : IDisplay
  {
    private DisplayRequest displayRequest = null;

    public void Activate()
    {
      if (displayRequest == null)
        displayRequest = new DisplayRequest();

      displayRequest.RequestActive();
    }

    public void Release()
    {
      displayRequest?.RequestRelease();
    }
  }
}