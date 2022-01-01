using System;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public class ArticleSectionEventArgs : EventArgs
  {
    public ArticleSectionViewModel Section { get; }

    public ArticleSectionEventArgs(ArticleSectionViewModel section)
    {
      Section = section;
    }
  }

  public sealed partial class ContentsView : UserControl
  {
    public event EventHandler<ArticleSectionEventArgs> ArticleSectionClick;

    public ContentsView()
    {
      InitializeComponent();
    }

    private void ContentsListViewItemClick(object sender, ItemClickEventArgs e)
    {
      if (e.ClickedItem is ArticleSectionViewModel section)
        ArticleSectionClick?.Invoke(this, new ArticleSectionEventArgs(section));
    }
  }
}