﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikipediaApp
{
  public class MapViewModel : ViewModelBase
  {
    private readonly WikipediaService wikipediaService = new WikipediaService();
    private readonly NavigationService navigationService = new NavigationService();
    private readonly GeolocationService geolocationService = new GeolocationService();

    private readonly string language;

    public MapViewModel(string language)
    {
      this.language = language;
    }

    private MapPosition position = null;
    private Command<MapPosition> movePositionCommand = null;
    private IList<NearbyArticle> articles = null;
    private NearbyArticle selectedArticle = null;
    private Command<NearbyArticle> selectArticleCommand = null;
    private Command<NearbyArticle> showArticleCommand = null;
    private AsyncCommand updateLocationCommand = null;

    public MapPosition Position
    {
      get { return position; }
      private set { SetProperty(ref position, value); }
    }

    public ICommand MovePositionCommand
    {
      get { return movePositionCommand ?? (movePositionCommand = new Command<MapPosition>(MovePosition)); }
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

    public ICommand SelectArticleCommand
    {
      get { return selectArticleCommand ?? (selectArticleCommand = new Command<NearbyArticle>(SelectArticle)); }
    }

    public ICommand ShowArticleCommand
    {
      get { return showArticleCommand ?? (showArticleCommand = new Command<NearbyArticle>(ShowArticle)); }
    }

    public ICommand UpdateLocationCommand
    {
      get { return updateLocationCommand ?? (updateLocationCommand = new AsyncCommand(UpdateLocation)); }
    }

    public override async Task Initialize()
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
