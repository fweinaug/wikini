using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace WikipediaApp
{
  public static class ListViewExtensions
  {
    public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
      "Command", typeof(ICommand), typeof(ListViewExtensions), new PropertyMetadata(null, OnCommandPropertyChanged));

    public static readonly DependencyProperty ItemMenuFlyoutProperty = DependencyProperty.RegisterAttached(
      "ItemMenuFlyout", typeof(MenuFlyout), typeof(ListViewExtensions), new PropertyMetadata(null, OnItemMenuFlyoutPropertyChanged));

    public static ICommand GetCommand(DependencyObject obj)
    {
      return (ICommand)obj.GetValue(CommandProperty);
    }

    public static void SetCommand(DependencyObject obj, ICommand value)
    {
      obj.SetValue(CommandProperty, value);
    }

    public static MenuFlyout GetItemMenuFlyout(DependencyObject obj)
    {
      return (MenuFlyout)obj.GetValue(ItemMenuFlyoutProperty);
    }

    public static void SetItemMenuFlyout(DependencyObject obj, MenuFlyout value)
    {
      obj.SetValue(ItemMenuFlyoutProperty, value);
    }

    private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var listView = (ListViewBase)d;

      listView.IsItemClickEnabled = false;
      listView.ItemClick -= OnListViewItemClicked;

      if (e.NewValue is ICommand)
      {
        listView.IsItemClickEnabled = true;
        listView.ItemClick += OnListViewItemClicked;
      }
    }

    private static void OnListViewItemClicked(object sender, ItemClickEventArgs e)
    {
      var listView = (ListViewBase)sender;
      var command = GetCommand(listView);

      if (command != null && command.CanExecute(e.ClickedItem))
        command.Execute(e.ClickedItem);
    }

    private static void OnItemMenuFlyoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var item = (FrameworkElement)d;

      item.IsRightTapEnabled = false;
      item.RightTapped -= OnListViewItemRightTapped;

      if (e.NewValue is MenuFlyout)
      {
        item.IsRightTapEnabled = true;
        item.RightTapped += OnListViewItemRightTapped;
      }
    }

    private static void OnListViewItemRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
      var element = (FrameworkElement)sender;
      var flyout = GetItemMenuFlyout(element);

      flyout.ShowAt(element, new FlyoutShowOptions { Position = e.GetPosition(element) });
    }
  }
}