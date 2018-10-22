using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WikipediaApp
{
  public static class ArticleHistory
  {
    public static IList<ArticleHead> All { get; private set; }

    public static bool IsEmpty
    {
      get { return !(session.Count > 0 || database.Count > 0); }
    }

    private static readonly ObservableCollection<ArticleHead> session = new ObservableCollection<ArticleHead>();
    private static readonly ObservableCollection<ArticleHead> database = new ObservableCollection<ArticleHead>();

    public static void AddArticle(Article article)
    {
      if (session.Any(x => x.Language == article.Language && x.PageId == article.PageId))
        return;

      var read = new ReadArticle
      {
        Date = DateTime.Now,
        Language = article.Language,
        PageId = article.PageId,
        Title = article.Title,
        Uri = article.Uri
      };

      using (var context = new WikipediaContext())
      {
        context.History.Add(read);

        context.SaveChanges();
      }

      database.Insert(0, article);
      session.Insert(0, article);
    }

    public static async Task Initialize()
    {
      InitializeSource();

      await Task.Run(() =>
      {
        using (var context = new WikipediaContext())
        {
          return context.History.OrderByDescending(x => x.Date).ToList();
        }
      }).ContinueWith(task =>
      {
        var history = task.Result;

        history.ForEach(database.Add);
      }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private static void InitializeSource()
    {
      void UpdateSource() { All = Settings.Current.HistorySession ? session : database; }

      UpdateSource();

      Settings.Current.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName == nameof(Settings.HistorySession))
          UpdateSource();
      };
    }

    public static async Task Clear()
    {
      await Task.Run(() =>
      {
        using (var context = new WikipediaContext())
        {
          context.RemoveRange(context.History);
          context.SaveChanges();
        }
      });

      database.Clear();
      session.Clear();
    }
  }
}