using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace WikipediaApp
{
  public static class TextBoxExtensions
  {
    public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.RegisterAttached(
      "EnterCommand", typeof(ICommand), typeof(TextBoxExtensions), new PropertyMetadata(null, OnEnterCommandPropertyChanged));

    public static ICommand GetEnterCommand(DependencyObject obj)
    {
      return (ICommand)obj.GetValue(EnterCommandProperty);
    }

    public static void SetEnterCommand(DependencyObject obj, ICommand value)
    {
      obj.SetValue(EnterCommandProperty, value);
    }

    private static void OnEnterCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var textBox = (TextBox)d;

      textBox.KeyUp -= TextBoxKeyUp;

      var command = e.NewValue as ICommand;

      if (command != null)
        textBox.KeyUp += TextBoxKeyUp;
    }

    private static void TextBoxKeyUp(object sender, KeyRoutedEventArgs e)
    {
      if (e.Key != VirtualKey.Enter)
        return;

      var textBox = (TextBox)sender;
      var command = GetEnterCommand(textBox);

      if (command != null && command.CanExecute(null))
        command.Execute(null);
    }
  }
}