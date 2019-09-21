using System;
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
  }
}