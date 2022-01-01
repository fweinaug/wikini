using System.Threading.Tasks;

namespace WikipediaApp
{
  public interface IGeolocationService
  {
    Task<Coordinates> GetCurrentLocation();
  }
}