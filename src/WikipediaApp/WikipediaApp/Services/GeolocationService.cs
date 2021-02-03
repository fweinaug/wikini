using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace WikipediaApp
{
  public class GeolocationService
  {
    public async Task<Coordinates> GetCurrentLocation()
    {
      var accessStatus = await Geolocator.RequestAccessAsync();
      if (accessStatus != GeolocationAccessStatus.Allowed)
        return null;

      var geolocator = new Geolocator();
      var geoposition = await geolocator.GetGeopositionAsync();

      return new Coordinates(geoposition.Coordinate.Point.Position.Latitude, geoposition.Coordinate.Point.Position.Longitude);
    }
  }
}