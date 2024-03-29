﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Collections;

namespace WikipediaApp
{
  public class HistoryArticleCollection : ObservableCollection<HistoryArticleCollection.Group>
  {
    public void AddArticles(IEnumerable<ReadArticle> articles)
    {
      var groupedArticles = articles.OrderBy(x => x.Date).GroupBy(x => x.Date.Date);

      foreach (var articleGroup in groupedArticles)
      {
        var group = GetGroup(articleGroup.Key);

        foreach (var article in articleGroup)
        {
          group.Insert(0, new HistoryArticleViewModel(article));
        }
      }
    }

    private Group GetGroup(DateTime date)
    {
      var group = this.FirstOrDefault(x => x.Key == date);

      if (group == null)
      {
        group = new Group(date);

        var index = this.TakeWhile(x => x.Key > date).Count();

        Insert(index, group);
      }

      return group;
    }

    public void RemoveArticle(ReadArticle article)
    {
      var group = this.FirstOrDefault(x => x.Key == article.Date.Date);

      if (group != null)
      {
        var readArticle = group.FirstOrDefault(x => x.Article.Id == article.Id);

        group.Remove(readArticle);

        if (group.Count == 0)
          Remove(group);
      }
    }

    public class Group : ObservableGroup<DateTime, HistoryArticleViewModel>
    {
      public Group(DateTime key) : base(key)
      {
      }
    }
  }
}