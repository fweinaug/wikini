﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public sealed partial class HistoryView : UserControl
  {
    public event EventHandler ArticleClick;

    public HistoryView()
    {
      InitializeComponent();
    }

    private void HistoryListViewItemClick(object sender, ItemClickEventArgs e)
    {
      ArticleClick?.Invoke(this, EventArgs.Empty);
    }

    private async void ClearHistoryClick(object sender, RoutedEventArgs e)
    {
      await ArticleHistory.Clear();
    }

    private void RemoveArticleClick(object sender, RoutedEventArgs e)
    {
      var article = ((FrameworkElement)e.OriginalSource).DataContext as ReadArticle;

      ArticleHistory.RemoveArticle(article);
    }
  }
}