using Windows.Networking.Connectivity;

namespace WikipediaApp
{
  public class DeviceService
  {
    public bool IsInternetConnectionUnrestricted()
    {
      var connectionCost = NetworkInformation.GetInternetConnectionProfile()?.GetConnectionCost();
      if (connectionCost == null)
        return false;

      return (connectionCost.NetworkCostType == NetworkCostType.Unrestricted || connectionCost.NetworkCostType == NetworkCostType.Unknown)
        && !connectionCost.Roaming;
    }
  }
}