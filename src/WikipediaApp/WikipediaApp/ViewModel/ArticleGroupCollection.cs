using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace WikipediaApp
{
  public class ArticleGroupCollection : ObservableCollection<ArticleGroup>
  {
    public void AddArticles(IEnumerable<ReadArticle> articles)
    {
      var groupedArticles = articles.OrderBy(x => x.Date).GroupBy(x => x.Date.Date);

      foreach (var articleGroup in groupedArticles)
      {
        var group = GetGroup(articleGroup.Key);

        foreach (var article in articleGroup)
        {
          group.Insert(0, article);
        }
      }
    }

    private ArticleGroup GetGroup(DateTime date)
    {
      var group = this.FirstOrDefault(x => (DateTime)x.Key == date);

      if (group == null)
      {
        group = new ArticleGroup { Key = date, Title = GetTitle(date) };

        var index = this.TakeWhile(x => (DateTime)x.Key > date).Count();

        Insert(index, group);
      }

      return group;
    }

    private static string GetTitle(DateTime date)
    {
      var diff = DateTime.Today - date;

      if (diff.Days > 0)
        return date.ToShortDateString();

      var resourceLoader = ResourceLoader.GetForCurrentView();
      var title = resourceLoader.GetString("ArticleGroupDateToday");

      return title;
    }
  }
}