using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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

    public static readonly DependencyProperty ItemSelectedCommandProperty = DependencyProperty.Register(
      nameof(ItemSelectedCommand), typeof(ICommand), typeof(SearchBox), new PropertyMetadata(null));

    public static readonly DependencyProperty LanguageSelectedCommandProperty = DependencyProperty.Register(
      nameof(LanguageSelectedCommand), typeof(ICommand), typeof(SearchBox), new PropertyMetadata(null));

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

    public ICommand LanguageSelectedCommand
    {
      get { return (ICommand)GetValue(LanguageSelectedCommandProperty); }
      set { SetValue(LanguageSelectedCommandProperty, value); }
    }

    public SearchBox()
    {
      InitializeComponent();
    }

    private void OnLanguageButtonClick(object sender, RoutedEventArgs e)
    {
      var favorites = ArticleLanguages.All.Where(x => x.IsFavorite || x == QueryLanguage).Reverse().ToList();

      if (favorites.Count > 1)
      {
        ShowLanguagesMenuFlyout(favorites);
      }
      else
      {
        SelectLanguage?.Invoke(this, e);
      }
    }

    private void ShowLanguagesMenuFlyout(List<Language> favorites)
    {
      var menuFlyout = (MenuFlyout)Resources["LanguagesMenuFlyout"];

      while (menuFlyout.Items[0] is MenuFlyoutItem item)
      {
        item.Click -= OnLanguageMenuFlyoutItemClick;

        menuFlyout.Items.RemoveAt(0);
      }

      foreach (var language in favorites)
      {
        var item = new MenuFlyoutItem
        {
          Text = language.Name,
          Icon = language == QueryLanguage ? new FontIcon {Glyph = "\uE73E"} : null,
          DataContext = language
        };

        item.Click += OnLanguageMenuFlyoutItemClick;

        menuFlyout.Items.Insert(0, item);
      }

      menuFlyout.ShowAt(LanguageButton, new FlyoutShowOptions { Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft });
    }

    private void OnLanguageMenuFlyoutItemClick(object sender, RoutedEventArgs e)
    {
      var item = (MenuFlyoutItem)sender;
      var language = item.DataContext as Language;
      
      var command = LanguageSelectedCommand;
      if (command != null && command.CanExecute(language))
        command.Execute(language);
    }

    private void OnMoreLanguagesMenuFlyoutItemClick(object sender, RoutedEventArgs e)
    {
      SelectLanguage?.Invoke(this, e);
    }

    private void OnAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
      if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput ||
          args.Reason == AutoSuggestionBoxTextChangeReason.ProgrammaticChange)
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

    private async void OnRecognizeSpeechClick(object sender, RoutedEventArgs e)
    {
      var text = await SpeechRecognitionHelper.RecognizeSearchTerm();

      if (!string.IsNullOrEmpty(text))
      {
        TextBox.Focus(FocusState.Programmatic);
        TextBox.Text = text;
      }
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