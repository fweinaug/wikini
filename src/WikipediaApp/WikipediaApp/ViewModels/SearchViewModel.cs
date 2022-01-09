using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace WikipediaApp
{
  public class SearchViewModel : ObservableObject
  {
    private readonly IWikipediaService wikipediaService;
    private readonly INavigationService navigationService;
    private readonly IDeviceService deviceService;
    private readonly IUserSettings userSettings;

    private LanguageViewModel language = null;
    private string searchTerm = null;
    private bool searchTermChanged = false;
    private IList<FoundArticle> searchResults = null;
    private bool noSearchResults = false;

    private RelayCommand searchCommand = null;
    private RelayCommand<ArticleHead> showArticleCommand = null;

    private CancellationTokenSource liveSearchCancellationTokenSource = null;
    private readonly object liveSearchLock = new object();

    public LanguageViewModel Language
    {
      get { return language; }
      set
      {
        if (SetProperty(ref language, value))
        {
          Search();
        }
      }
    }

    public string SearchTerm
    {
      get { return searchTerm; }
      set
      {
        if (SetProperty(ref searchTerm, value))
        {
          searchTermChanged = true;
          searchCommand?.NotifyCanExecuteChanged();

          if (userSettings.Get<bool>(UserSettingsKey.SearchRestricted) || deviceService.IsInternetConnectionUnrestricted())
            LiveSearch();
        }
      }
    }

    public ICommand SearchCommand
    {
      get { return searchCommand ?? (searchCommand = new RelayCommand(SearchIfTermChanged, CanSearch)); }
    }

    public IList<FoundArticle> SearchResults
    {
      get { return searchResults; }
      private set
      {
        if (SetProperty(ref searchResults, value))
          NoSearchResults = !string.IsNullOrEmpty(searchTerm) && (searchResults == null || searchResults.Count == 0);
      }
    }

    public bool NoSearchResults
    {
      get { return noSearchResults; }
      private set { SetProperty(ref noSearchResults, value); }
    }

    public ICommand ShowArticleCommand
    {
      get { return showArticleCommand ?? (showArticleCommand = new RelayCommand<ArticleHead>(ShowArticle)); }
    }

    public SearchViewModel(IWikipediaService wikipediaService, INavigationService navigationService, IDeviceService deviceService, IUserSettings userSettings)
    {
      this.wikipediaService = wikipediaService;
      this.navigationService = navigationService;
      this.deviceService = deviceService;
      this.userSettings = userSettings;
    }

    public void ShowArticle(ArticleHead article)
    {
      ClearSearch();

      navigationService.ShowArticle(article);
    }

    private void ClearSearch()
    {
      SearchTerm = null;
      SearchResults = null;
    }

    public async void Search()
    {
      if (string.IsNullOrWhiteSpace(searchTerm))
      {
        SearchResults = null;
      }
      else
      {
        var results = await wikipediaService.Search(searchTerm, language.Code, null);
        if (results == null)
          return;

        if (ContainsNewArticles(searchResults, results))
          SearchResults = results;
      }

      searchTermChanged = false;
    }

    private void SearchIfTermChanged()
    {
      if (!searchTermChanged)
        return;

      Search();
    }

    private bool CanSearch()
    {
      return !string.IsNullOrEmpty(searchTerm);
    }

    private async void LiveSearch()
    {
      await Task.Delay(100);

      if (liveSearchCancellationTokenSource != null)
      {
        liveSearchCancellationTokenSource.Cancel();
        liveSearchCancellationTokenSource = null;
      }

      var term = searchTerm;

      if (string.IsNullOrWhiteSpace(term))
      {
        SearchResults = null;
      }
      else
      {
        liveSearchCancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = liveSearchCancellationTokenSource.Token;

        var results = await wikipediaService.Search(term, language.Code, cancellationToken);
        if (results == null || cancellationToken.IsCancellationRequested)
          return;

        lock (liveSearchLock)
        {
          if (ContainsNewArticles(searchResults, results))
            SearchResults = results;
        }
      }
    }

    private static bool ContainsNewArticles(IList<FoundArticle> currentArticles, IList<FoundArticle> foundArticles)
    {
      if (currentArticles == null || foundArticles == null)
        return !Equals(currentArticles, foundArticles);

      if (currentArticles.Count != foundArticles.Count)
        return true;

      for (var i = 0; i < currentArticles.Count; ++i)
      {
        if (currentArticles[i].Uri != foundArticles[i].Uri)
          return true;
      }

      return false;
    }
  }
}