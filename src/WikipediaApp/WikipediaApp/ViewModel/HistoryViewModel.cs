using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace WikipediaApp
{
  #region Messages

  public class AddArticleToHistory
  {
    public ArticleHead Article { get; private set; }

    public AddArticleToHistory(ArticleHead article)
    {
      Article = article;
    }
  }

  public class RemoveArticleFromHistory
  {
    public ReadArticle Article { get; private set; }

    public RemoveArticleFromHistory(ReadArticle article)
    {
      Article = article;
    }
  }

  public class ClearHistory : AsyncRequestMessage<bool>
  {
  }

  public class IsHistoryEmpty : RequestMessage<bool>
  {
  }

  #endregion

  public class HistoryViewModel : ViewModelBase
  {
    private readonly INavigationService navigationService;

    private readonly ArticleGroupCollection groupedArticles = new();
    private readonly ObservableCollection<ReadArticle> session = new();
    private readonly ObservableCollection<ReadArticle> database = new();

    private RelayCommand<HistoryArticleViewModel> showArticleCommand;

    public ArticleGroupCollection All
    {
      get { return groupedArticles; }
    }

    public ICommand ShowArticleCommand
    {
      get { return showArticleCommand ??= new RelayCommand<HistoryArticleViewModel>(ShowArticle); }
    }

    public bool IsEmpty
    {
      get { return !(session.Count > 0 || database.Count > 0); }
    }

    public HistoryViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;

      WeakReferenceMessenger.Default.Register<HistoryViewModel, AddArticleToHistory>(this, (_, message) =>
      {
        AddArticle(message.Article);
      });
      WeakReferenceMessenger.Default.Register<HistoryViewModel, RemoveArticleFromHistory>(this, (_, message) =>
      {
        RemoveArticle(message.Article);
      });
      WeakReferenceMessenger.Default.Register<HistoryViewModel, ClearHistory>(this, (_, message) =>
      {
        message.Reply(ClearHistory());
      });
      WeakReferenceMessenger.Default.Register<HistoryViewModel, IsHistoryEmpty>(this, (_, message) =>
      {
        message.Reply(IsEmpty);
      });
    }

    public override async Task Initialize()
    {
      var history = await ArticleHistory.GetHistory();

      history.ForEach(database.Add);

      InitializeSource();
    }

    private void AddArticle(ArticleHead article)
    {
      if (session.Any(x => x.Language == article.Language && x.PageId == article.PageId))
        return;

      var read = ArticleHistory.AddArticle(article);

      database.Insert(0, read);
      session.Insert(0, read);
    }

    private void RemoveArticle(ReadArticle article)
    {
      if (database.FirstOrDefault(x => x.Id == article.Id) is ReadArticle readArticle)
      {
        ArticleHistory.RemoveArticle(article);

        database.Remove(readArticle);
      }

      session.Remove(article);

      groupedArticles.RemoveArticle(article);
    }

    private async Task<bool> ClearHistory()
    {
      await ArticleHistory.Clear();

      database.Clear();
      session.Clear();

      return true;
    }

    private void ShowArticle(HistoryArticleViewModel article)
    {
      navigationService.ShowArticle(article.Article);
    }

    private void InitializeSource()
    {
      session.CollectionChanged += SessionCollectionChanged;

      void UpdateSource()
      {
        var articles = Settings.Current.HistorySession ? session : database;

        groupedArticles.Clear();
        groupedArticles.AddArticles(articles);
      }

      UpdateSource();

      Settings.Current.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName == nameof(Settings.HistorySession))
          UpdateSource();
      };
    }

    private void SessionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Reset:
          groupedArticles.Clear();
          break;
        case NotifyCollectionChangedAction.Add:
          groupedArticles.AddArticles(e.NewItems.Cast<ReadArticle>());
          break;
      }
    }
  }
}