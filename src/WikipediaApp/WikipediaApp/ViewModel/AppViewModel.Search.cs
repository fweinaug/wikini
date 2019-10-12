using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikipediaApp
{
  public partial class AppViewModel
  {
    private string searchTerm = null;
    private bool searchTermChanged = false;
    private IList<FoundArticle> searchResults = null;
    private bool noSearchResults = false;

    private Command searchCommand = null;

    private CancellationTokenSource liveSearchCancellationTokenSource = null;
    private readonly object liveSearchLock = new object();

    public string SearchTerm
    {
      get { return searchTerm; }
      set
      {
        if (SetProperty(ref searchTerm, value))
        {
          searchTermChanged = true;
          searchCommand?.RaiseCanExecuteChanged();

          if (Settings.Current.SearchRestricted || deviceService.IsInternetConnectionUnrestricted())
            LiveSearch();
        }
      }
    }

    public ICommand SearchCommand
    {
      get { return searchCommand ?? (searchCommand = new Command(SearchIfTermChanged, CanSearch)); }
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

    private void SearchIfTermChanged()
    {
      if (!searchTermChanged)
        return;

      Search();
    }

    private async void Search()
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

    private void ClearSearch()
    {
      SearchTerm = null;
      SearchResults = null;
    }
  }
}