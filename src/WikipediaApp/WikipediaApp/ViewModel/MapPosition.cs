using System;

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
}