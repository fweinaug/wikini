using System;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace WikipediaApp
{
  public sealed partial class HistoryView : UserControl
  {
    public event EventHandler ArticleClick;

    public HistoryView()
    {
      InitializeComponent();

      DataContext = App.Services.GetService<HistoryViewModel>();
    }

    private void HistoryListViewItemClick(object sender, ItemClickEventArgs e)
    {
      ArticleClick?.Invoke(this, EventArgs.Empty);
    }
  }
}