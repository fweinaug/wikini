using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Microsoft.HockeyApp;

namespace WikipediaApp
{
  public static class InAppPurchases
  {
#if DEBUG
    public static async Task ConfigureSimulator()
    {
      var uri = new Uri("ms-appx:///Data/in-app-purchases.xml");
      var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

      await CurrentAppSimulator.ReloadSimulatorAsync(file);
    }
#endif

    public static async Task<bool> Donation1()
    {
      return await PurchaseAndFulfillConsumable("WikipediaApp_Donate1");
    }

    public static async Task<bool> Donation2()
    {
      return await PurchaseAndFulfillConsumable("WikipediaApp_Donate2");
    }

    public static async Task<bool> Donation3()
    {
      return await PurchaseAndFulfillConsumable("WikipediaApp_Donate3");
    }

    private static async Task<bool> PurchaseAndFulfillConsumable(string productId)
    {
      try
      {
#if DEBUG
        var result = await CurrentAppSimulator.RequestProductPurchaseAsync(productId);
#else
        var result = await CurrentApp.RequestProductPurchaseAsync(productId);
#endif

        switch (result.Status)
        {
          case ProductPurchaseStatus.Succeeded:
          case ProductPurchaseStatus.NotFulfilled:
            return await FulfillConsumable(productId, result.TransactionId);
          default:
            return false;
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    private static async Task<bool> FulfillConsumable(string productId, Guid transactionId)
    {
      try
      {
#if DEBUG
        var result = await CurrentAppSimulator.ReportConsumableFulfillmentAsync(productId, transactionId);
#else
        var result = await CurrentApp.ReportConsumableFulfillmentAsync(productId, transactionId);
#endif

        return result == FulfillmentResult.Succeeded;
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return false;
      }
    }
  }
}