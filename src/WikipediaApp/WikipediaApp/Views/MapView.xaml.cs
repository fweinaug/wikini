using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;

namespace WikipediaApp
{
  public sealed partial class MapView : UserControl
  {
    public static readonly DependencyProperty ArticleTemplateProperty = DependencyProperty.Register(
      nameof(ArticleTemplate), typeof(DataTemplate), typeof(MapView), new PropertyMetadata(null));

    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
      nameof(Position), typeof(MapPosition), typeof(MapView), new PropertyMetadata(null, OnPositionPropertyChanged));

    public static readonly DependencyProperty MovePositionCommandProperty = DependencyProperty.Register(
      nameof(MovePositionCommand), typeof(ICommand), typeof(MapView), new PropertyMetadata(null));

    public static readonly DependencyProperty ArticlesProperty = DependencyProperty.Register(
      nameof(Articles), typeof(IList<NearbyArticle>), typeof(MapView), new PropertyMetadata(null, OnArticlesPropertyChanged));

    public static readonly DependencyProperty SelectedArticleProperty = DependencyProperty.Register(
      nameof(SelectedArticle), typeof(NearbyArticle), typeof(MapView), new PropertyMetadata(null, OnSelectedArticlePropertyChanged));

    public static readonly DependencyProperty SelectArticleCommandProperty = DependencyProperty.Register(
      nameof(SelectArticleCommand), typeof(ICommand), typeof(MapView), new PropertyMetadata(null));

    public static readonly DependencyProperty ShowArticleCommandProperty = DependencyProperty.Register(
      nameof(ShowArticleCommand), typeof(ICommand), typeof(MapView), new PropertyMetadata(null));

    public static readonly DependencyProperty UpdateLocationCommandProperty = DependencyProperty.Register(
      nameof(UpdateLocationCommand), typeof(ICommand), typeof(MapView), new PropertyMetadata(null));

    public DataTemplate ArticleTemplate
    {
      get { return (DataTemplate)GetValue(ArticleTemplateProperty); }
      set { SetValue(ArticleTemplateProperty, value); }
    }

    public MapPosition Position
    {
      get { return (MapPosition)GetValue(PositionProperty); }
      set { SetValue(PositionProperty, value); }
    }

    public ICommand MovePositionCommand
    {
      get { return (ICommand)GetValue(MovePositionCommandProperty); }
      set { SetValue(MovePositionCommandProperty, value); }
    }

    public IList<NearbyArticle> Articles
    {
      get { return (IList<NearbyArticle>)GetValue(ArticlesProperty); }
      set { SetValue(ArticlesProperty, value); }
    }

    public NearbyArticle SelectedArticle
    {
      get { return (NearbyArticle)GetValue(SelectedArticleProperty); }
      set { SetValue(SelectedArticleProperty, value); }
    }

    public ICommand SelectArticleCommand
    {
      get { return (ICommand)GetValue(SelectArticleCommandProperty); }
      set { SetValue(SelectArticleCommandProperty, value); }
    }

    public ICommand ShowArticleCommand
    {
      get { return (ICommand)GetValue(ShowArticleCommandProperty); }
      set { SetValue(ShowArticleCommandProperty, value); }
    }

    public ICommand UpdateLocationCommand
    {
      get { return (ICommand)GetValue(UpdateLocationCommandProperty); }
      set { SetValue(UpdateLocationCommandProperty, value); }
    }

    public MapView()
    {
      InitializeComponent();
    }

    private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (MapView)d;

      control.UpdateMapPosition();
    }

    private static void OnArticlesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (MapView)d;

      control.UpdateMapElements();
    }

    private static void OnSelectedArticlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (MapView)d;

      control.ShowArticlePopup();
    }

    private async void UpdateMapPosition()
    {
      var geopoint = new Geopoint(new BasicGeoposition
      {
        Latitude = Position.Latitude, Longitude = Position.Longitude, Altitude = Position.Altitude
      });

      await MapControl.TrySetViewAsync(geopoint, Position.ZoomLevel, null, null, MapAnimationKind.Default);
    }

    private void UpdateMapElements()
    {
      var currentElements = MapControl.MapElements.ToDictionary(x => ((NearbyArticle)x.Tag).PageId.Value);

      foreach (var article in Articles)
      {
        if (currentElements.ContainsKey(article.PageId.Value))
        {
          currentElements.Remove(article.PageId.Value);
          continue;
        }

        MapControl.MapElements.Add(new MapIcon
        {
          Title = article.Title,
          Location = new Geopoint(new BasicGeoposition { Latitude = article.Coordinates.Latitude, Longitude = article.Coordinates.Longitude }),
          NormalizedAnchorPoint = new Point(0.5, 1.0),
          CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
          Tag = article
        });
      }

      foreach (var element in currentElements.Values)
      {
        MapControl.MapElements.Remove(element);
      }
    }

    private void ShowArticlePopup()
    {
      ArticlePopupContentPresenter.Content = SelectedArticle;
      ArticlePopup.IsOpen = SelectedArticle != null;
    }

    private void MapControlLoadingStatusChanged(MapControl sender, object args)
    {
      if (sender.LoadingStatus != MapLoadingStatus.Loaded)
        return;

      var position = new MapPosition
      {
        Latitude = sender.Center.Position.Latitude,
        Longitude = sender.Center.Position.Longitude,
        Altitude = sender.Center.Position.Altitude,
        ZoomLevel = sender.ZoomLevel
      };

      MovePositionCommand?.Execute(position);
    }

    private void MapControlMapTapped(MapControl sender, MapInputEventArgs args)
    {
      var elements = MapControl.FindMapElementsAtOffset(args.Position);
      var article = elements.Count > 0 ? (NearbyArticle)elements[0].Tag : null;

      SelectArticleCommand?.Execute(article);
    }

    private void ArticlePopupContentPresenterSizeChanged(object sender, SizeChangedEventArgs e)
    {
      ArticlePopup.HorizontalOffset = 15;
      ArticlePopup.VerticalOffset = -(e.NewSize.Height + 15);
    }

    private void ArticlePopupContentPresenterTapped(object sender, TappedRoutedEventArgs e)
    {
      ShowArticleCommand?.Execute(ArticlePopupContentPresenter.Content);
    }

    private void ArticlePopupContentPresenterPointerPressed(object sender, PointerRoutedEventArgs e)
    {
      ShowArticleCommand?.Execute(ArticlePopupContentPresenter.Content);
    }

    private async void ZoomInButtonClick(object sender, RoutedEventArgs e)
    {
      await MapControl.TryZoomInAsync();
    }

    private async void ZoomOutButtonClick(object sender, RoutedEventArgs e)
    {
      await MapControl.TryZoomOutAsync();
    }
  }
}