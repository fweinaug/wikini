using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace WikipediaApp
{
  public class HistoryArticleViewModel : ObservableObject
  {
    private RelayCommand removeFromHistoryCommand;
    private RelayCommand clearHistoryCommand;

    public ReadArticle Article { get; }

    public string Title => Article.Title;
    public string Language => Article.Language;
    public string Description => Article.Description;
    public bool HasDescription => Article.HasDescription;

    public ICommand RemoveFromHistoryCommand
    {
      get { return removeFromHistoryCommand ??= new RelayCommand(RemoveFromHistory); }
    }

    public ICommand ClearHistoryCommand
    {
      get { return clearHistoryCommand ??= new RelayCommand(ClearHistory); }
    }

    public HistoryArticleViewModel(ReadArticle article)
    {
      Article = article;
    }

    private void RemoveFromHistory()
    {
      WeakReferenceMessenger.Default.Send(new RemoveArticleFromHistory(Article));
    }

    private void ClearHistory()
    {
      WeakReferenceMessenger.Default.Send(new ClearHistory());
    }
  }
}