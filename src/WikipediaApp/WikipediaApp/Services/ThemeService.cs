namespace WikipediaApp
{
  public class ThemeService
  {
    public bool IsDarkMode()
    {
      var appTheme = Settings.Current.AppTheme;

      var darkMode = appTheme == 2 ||
                     appTheme == 0 && App.Current.RequestedTheme == Windows.UI.Xaml.ApplicationTheme.Dark;

      return darkMode;
    }
  }
}