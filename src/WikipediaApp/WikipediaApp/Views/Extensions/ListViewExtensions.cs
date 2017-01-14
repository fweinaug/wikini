using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public static class ListViewExtensions
  {
    public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
      "Command", typeof(ICommand), typeof(ListViewExtensions), new PropertyMetadata(null, OnCommandPropertyChanged));

    public static ICommand GetCommand(DependencyObject obj)
    {
      return (ICommand)obj.GetValue(CommandProperty);
    }

    public static void SetCommand(DependencyObject obj, ICommand value)
    {
      obj.SetValue(CommandProperty, value);
    }

    private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var listView = (ListViewBase)d;

      listView.IsItemClickEnabled = false;
      listView.ItemClick -= OnListViewItemClicked;

      var command = e.NewValue as ICommand;

      if (command != null)
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
  }
}