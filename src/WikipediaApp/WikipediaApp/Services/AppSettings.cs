namespace WikipediaApp
{
  public class AppSettings : IAppSettings
  {
    public bool DarkMode => App.Current.InDarkMode();
  }
}
