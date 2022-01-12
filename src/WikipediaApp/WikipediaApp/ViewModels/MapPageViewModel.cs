using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace WikipediaApp
{
  public class MapPosition
  {
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public double Altitude { get; set; }
    public double ZoomLevel { get; set; }

    public bool SameLocation(MapPosition position)
    {
      return position != null && Math.Abs(Latitude - position.Latitude) < 0.000001 && Math.Abs(Longitude - position.Longitude) < 0.000001;
    }
  }

  public class MapPageViewModel : ObservableObject
  {
    private readonly IWikipediaService wikipediaService;
    private readonly INavigationService navigationService;
    private readonly IGeolocationService geolocationService;

    private string language;
    private MapPosition position = null;
    private IList<NearbyArticle> articles = null;
    private NearbyArticle selectedArticle = null;

    private AsyncRelayCommand loadCommand = null;
    private RelayCommand<MapPosition> movePositionCommand = null;
    private RelayCommand<NearbyArticle> selectArticleCommand = null;
    private RelayCommand<NearbyArticle> showArticleCommand = null;
    private AsyncRelayCommand updateLocationCommand = null;

    public string Language
    {
      get { return language; }
      set { SetProperty(ref language, value); }
    }

    public MapPosition Position
    {
      get { return position; }
      private set { SetProperty(ref position, value); }
    }

    public IList<NearbyArticle> Articles
    {
      get { return articles; }
      private set { SetProperty(ref articles, value); }
    }

    public NearbyArticle SelectedArticle
    {
      get { return selectedArticle; }
      private set { SetProperty(ref selectedArticle, value); }
    }

    public AsyncRelayCommand LoadCommand
    {
      get { return loadCommand ?? (loadCommand = new AsyncRelayCommand(Initialize)); }
    }

    public ICommand MovePositionCommand
    {
      get { return movePositionCommand ?? (movePositionCommand = new RelayCommand<MapPosition>(MovePosition)); }
    }

    public ICommand SelectArticleCommand
    {
      get { return selectArticleCommand ?? (selectArticleCommand = new RelayCommand<NearbyArticle>(SelectArticle)); }
    }

    public ICommand ShowArticleCommand
    {
      get { return showArticleCommand ?? (showArticleCommand = new RelayCommand<NearbyArticle>(ShowArticle)); }
    }

    public ICommand UpdateLocationCommand
    {
      get { return updateLocationCommand ?? (updateLocationCommand = new AsyncRelayCommand(UpdateLocation)); }
    }

    public MapPageViewModel(IWikipediaService wikipediaService, INavigationService navigationService, IGeolocationService geolocationService)
    {
      this.wikipediaService = wikipediaService;
      this.navigationService = navigationService;
      this.geolocationService = geolocationService;
    }

    private async Task Initialize()
    {
      if (Position == null)
        await UpdateLocation();
    }

    private void MovePosition(MapPosition newPosition)
    {
      if (!newPosition.SameLocation(position))
        Search(newPosition);
      
      position = newPosition;
    }

    private async void SelectArticle(NearbyArticle article)
    {
      SelectedArticle = article != null
        ? await wikipediaService.GetArticleLocation(article.Language, article.PageId, article.Title)
        : null;
    }

    private void ShowArticle(NearbyArticle article)
    {
      navigationService.ShowArticle(article);
    }

    private async Task UpdateLocation()
    {
      var location = await geolocationService.GetCurrentLocation();
      if (location == null)
        return;

      Position = new MapPosition
      {
        Latitude = location.Latitude,
        Longitude = location.Longitude,
        ZoomLevel = 14
      };

      Search(position);
    }

    private async void Search(MapPosition position)
    {
      Articles = await wikipediaService.SearchNearby(language, position.Latitude, position.Longitude);
    }
  }
}
