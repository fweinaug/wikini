using System;
using System.Windows.Input;

namespace WikipediaApp
{
  public class ReadArticle : ArticleHead
  {
    public int Id { get; set; }
    public DateTime Date { get; set; }

    private ICommand clearHistoryCommand = null;

    public ICommand ClearHistoryCommand
    {
      get { return clearHistoryCommand ?? (clearHistoryCommand = new Command(ClearHistory)); }
    }

    private async void ClearHistory()
    {
      await ArticleHistory.Clear();
    }
  }
}