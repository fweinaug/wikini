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
      nameof(QueryLanguage), typeof(LanguageViewModel), typeof(SearchBox), new PropertyMetadata(null));

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

    public static readonly DependencyProperty LanguagesProperty = DependencyProperty.Register(
      nameof(Languages), typeof(IList<LanguageViewModel>), typeof(SearchBox), new PropertyMetadata(null));

    public LanguageViewModel QueryLanguage
    {
      get { return (LanguageViewModel)GetValue(QueryLanguageProperty); }
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

    public IList<LanguageViewModel> Languages
    {
      get { return (IList<LanguageViewModel>)GetValue(LanguagesProperty); }
      set { SetValue(LanguagesProperty, value); }
    }

    public SearchBox()
    {
      InitializeComponent();
    }

    private void OnLanguageButtonClick(object sender, RoutedEventArgs e)
    {
      var languages = Languages.Where(language => language is not EmptyFavoritesHint).ToList();
      if (!languages.Contains(QueryLanguage))
        languages.Add(QueryLanguage);
      languages.Sort((x, y) => -x.Index.CompareTo(y.Index));

      if (languages.Count > 1)
      {
        ShowLanguagesMenuFlyout(languages);
      }
      else
      {
        SelectLanguage?.Invoke(this, e);
      }
    }

    private void ShowLanguagesMenuFlyout(List<LanguageViewModel> favorites)
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
          Icon = language == QueryLanguage ? new FontIcon { Glyph="\uF96C" } : null,
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
      var language = item.DataContext as LanguageViewModel;
      
      LanguageSelectedCommand?.Execute(language);
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

        ItemSelectedCommand?.Execute(item);
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
      QueryCommand?.Execute(null);
      
      if (TextBox.Items?.Count > 0)
        TextBox.IsSuggestionListOpen = true;
    }
  }
}