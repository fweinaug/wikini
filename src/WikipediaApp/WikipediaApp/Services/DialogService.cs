namespace WikipediaApp
{
  public class DialogService
  {
    public async void ShowLoadingError()
    {
      var shell = App.Current.AppShell;

      await shell.ShowLoadingError();
    }
  }
}