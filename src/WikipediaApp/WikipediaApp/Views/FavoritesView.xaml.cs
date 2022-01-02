using System;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace WikipediaApp
{
  public sealed partial class FavoritesView : UserControl
  {
    public event EventHandler ArticleClick;

    public FavoritesView()
    {
      InitializeComponent();

      DataContext = App.Services.GetService<FavoritesViewModel>();
    }

    private void FavoritesListViewItemClick(object sender, ItemClickEventArgs e)
    {
      ArticleClick?.Invoke(this, EventArgs.Empty);
    }
  }
}