namespace WikipediaApp
{
  public class DialogService : IDialogService
  {
    public async void ShowLoadingError()
    {
      var shell = App.Current.AppShell;

      await shell.ShowLoadingError();
    }
  }
}