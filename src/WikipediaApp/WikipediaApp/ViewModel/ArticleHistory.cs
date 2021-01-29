using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WikipediaApp
{
  public static class ArticleHistory
  {
    private static readonly ObservableCollection<ReadArticle> session = new ObservableCollection<ReadArticle>();
    private static readonly ObservableCollection<ReadArticle> database = new ObservableCollection<ReadArticle>();
    private static readonly ArticleGroupCollection groupedArticles = new ArticleGroupCollection();

    public static IList<ArticleGroup> All
    {
      get { return groupedArticles; }
    }

    public static bool IsEmpty
    {
      get { return !(session.Count > 0 || database.Count > 0); }
    }

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
        Description = article.Description,
        Uri = article.Uri,
        ThumbnailUri = article.ThumbnailUri
      };

      using (var context = new WikipediaContext())
      {
        context.History.Add(read);

        context.SaveChanges();
      }

      database.Insert(0, read);
      session.Insert(0, read);
    }

    public static async Task Initialize()
    {
      using (var context = new WikipediaContext())
      {
        var history = await context.History.OrderByDescending(x => x.Date).ToListAsync();

        history.ForEach(database.Add);

        InitializeSource();
      }
    }

    private static void InitializeSource()
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

    private static void SessionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

    public static async Task Clear()
    {
      using (var context = new WikipediaContext())
      {
        context.RemoveRange(context.History);
        await context.SaveChangesAsync();
      }

      database.Clear();
      session.Clear();
    }

    public static void RemoveArticle(ReadArticle article)
    {
      if (database.FirstOrDefault(x => x.Id == article.Id) is ReadArticle readArticle)
      {
        using (var context = new WikipediaContext())
        {
          context.History.Remove(readArticle);
          context.SaveChanges();
        }

        database.Remove(readArticle);
      }

      session.Remove(article);

      groupedArticles.RemoveArticle(article);
    }
  }
}