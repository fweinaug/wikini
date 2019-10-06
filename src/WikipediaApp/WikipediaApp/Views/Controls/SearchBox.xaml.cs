using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace WikipediaApp
{
  public sealed partial class SearchBox : UserControl
  {
    public event RoutedEventHandler SelectLanguage;

    public static readonly DependencyProperty QueryLanguageProperty = DependencyProperty.Register(
      nameof(QueryLanguage), typeof(Language), typeof(SearchBox), new PropertyMetadata(null));

    public static readonly DependencyProperty QueryStringProperty = DependencyProperty.Register(
      nameof(QueryString), typeof(string), typeof(SearchBox), new PropertyMetadata(null));

    public static readonly DependencyProperty QueryCommandProperty = DependencyProperty.Register(
      nameof(QueryCommand), typeof(ICommand), typeof(SearchBox), new PropertyMetadata(null));

    public static readonly DependencyProperty QueryResultsProperty = DependencyProperty.Register(
      nameof(QueryResults), typeof(object), typeof(SearchBox), new PropertyMetadata(null));

    public static readonly DependencyProperty ItemSelectedCommandProperty = DependencyProperty.RegisterAttached(
      nameof(ItemSelectedCommand), typeof(ICommand), typeof(SearchBox), new PropertyMetadata(null));

    public Language QueryLanguage
    {
      get { return (Language)GetValue(QueryLanguageProperty); }
      set { SetValue(QueryLanguageProperty, value); }
    }

    public string QueryString
    {
      get { return (string)GetValue(QueryStringProperty); }
      set { SetValue(QueryStringProperty, value); }
    }

    public ICommand QueryCommand
    {
      get { return (ICommand)GetValue(QueryCommandProperty); }
      set { SetValue(QueryCommandProperty, value); }
    }

    public object QueryResults
    {
      get { return GetValue(QueryResultsProperty); }
      set { SetValue(QueryResultsProperty, value); }
    }

    public ICommand ItemSelectedCommand
    {
      get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
      set { SetValue(ItemSelectedCommandProperty, value); }
    }

    public SearchBox()
    {
      InitializeComponent();
    }

    private void OnLanguageButtonClick(object sender, RoutedEventArgs e)
    {
      SelectLanguage?.Invoke(this, e);
    }

    private void OnAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
      if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
      {
        QueryString = sender.Text;

        SearchButton.IsEnabled = !string.IsNullOrEmpty(sender.Text);
      }
    }

    private void OnAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
      if (args.ChosenSuggestion != null)
      {
        var item = args.ChosenSuggestion;
        var command = ItemSelectedCommand;

        if (command != null && command.CanExecute(item))
          command.Execute(item);
      }
    }

    private void OnAutoSuggestBoxKeyUp(object sender, KeyRoutedEventArgs e)
    {
      if (e.Key == VirtualKey.Enter)
        PerformQuery();
    }

    private void OnSearchButtonClick(object sender, RoutedEventArgs e)
    {
      PerformQuery();
    }

    private void PerformQuery()
    {
      var command = QueryCommand;

      if (command != null && command.CanExecute(null))
        command.Execute(null);
      
      if (TextBox.Items?.Count > 0)
        TextBox.IsSuggestionListOpen = true;
    }
  }
}