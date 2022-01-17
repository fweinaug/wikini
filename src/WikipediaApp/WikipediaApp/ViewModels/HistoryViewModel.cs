using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace WikipediaApp
{
  #region Messages

  public sealed class AddArticleToHistory
  {
    public ArticleHead Article { get; private set; }

    public AddArticleToHistory(ArticleHead article)
    {
      Article = article;
    }
  }

  public sealed class RemoveArticleFromHistory
  {
    public ReadArticle Article { get; private set; }

    public RemoveArticleFromHistory(ReadArticle article)
    {
      Article = article;
    }
  }

  public sealed class ClearHistory : AsyncRequestMessage<bool>
  {
  }

  public sealed class IsHistoryEmpty : RequestMessage<bool>
  {
  }

  public sealed class HistoryChanged
  {
  }

  #endregion

  public class HistoryViewModel : ObservableObject
  {
    private readonly INavigationService navigationService;
    private readonly IArticleHistoryRepository articleHistoryRepository;
    private readonly IUserSettings userSettings;

    private readonly HistoryArticleCollection groupedArticles = new();
    private readonly ObservableCollection<ReadArticle> session = new();
    private readonly ObservableCollection<ReadArticle> database = new();

    private RelayCommand<HistoryArticleViewModel> showArticleCommand;

    public HistoryArticleCollection All
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

    public HistoryViewModel(INavigationService navigationService, IArticleHistoryRepository articleHistoryRepository, IUserSettings userSettings)
    {
      this.navigationService = navigationService;
      this.articleHistoryRepository = articleHistoryRepository;
      this.userSettings = userSettings;

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

    public async Task Initialize()
    {
      var history = await articleHistoryRepository.GetHistory();

      history.ForEach(database.Add);

      InitializeSource();
    }

    private void AddArticle(ArticleHead article)
    {
      if (session.Any(x => x.Language == article.Language && x.PageId == article.PageId))
        return;

      var read = articleHistoryRepository.AddArticle(article);

      database.Insert(0, read);
      session.Insert(0, read);

      WeakReferenceMessenger.Default.Send<HistoryChanged>();
    }

    private void RemoveArticle(ReadArticle article)
    {
      if (database.FirstOrDefault(x => x.Id == article.Id) is ReadArticle readArticle)
      {
        articleHistoryRepository.RemoveArticle(article);

        database.Remove(readArticle);
      }

      session.Remove(article);

      groupedArticles.RemoveArticle(article);

      WeakReferenceMessenger.Default.Send<HistoryChanged>();
    }

    private async Task<bool> ClearHistory()
    {
      await articleHistoryRepository.Clear();

      database.Clear();
      session.Clear();

      WeakReferenceMessenger.Default.Send<HistoryChanged>();

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
        var articles = userSettings.Get<bool>(UserSettingsKey.HistorySession) ? session : database;

        groupedArticles.Clear();
        groupedArticles.AddArticles(articles);
      }

      UpdateSource();

      userSettings.SettingSet += (sender, settingKey) =>
      {
        if (settingKey == UserSettingsKey.HistorySession)
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