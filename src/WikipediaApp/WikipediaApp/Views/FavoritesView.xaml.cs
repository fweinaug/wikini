using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

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

    private void FavoritesListViewItemRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
      var element = (FrameworkElement)sender;
      var flyout = FlyoutBase.GetAttachedFlyout(element);

      flyout.ShowAt(element, new FlyoutShowOptions { Position = e.GetPosition(element) });
    }
  }
}