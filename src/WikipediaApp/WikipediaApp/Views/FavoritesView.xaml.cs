using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public sealed partial class FavoritesView : UserControl
  {
    public event EventHandler ArticleClick;

    public FavoritesView()
    {
      InitializeComponent();
    }

    private void FavoritesListViewItemClick(object sender, ItemClickEventArgs e)
    {
      ArticleClick?.Invoke(this, EventArgs.Empty);
    }

    private void RemoveArticleClick(object sender, RoutedEventArgs e)
    {
      var article = ((FrameworkElement)e.OriginalSource).DataContext as ArticleHead;

      ArticleFavorites.RemoveArticle(article);
    }
  }
}