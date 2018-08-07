using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Newtonsoft.Json;

namespace WikipediaApp
{
  public static class TileManager
  {
    public static async Task<bool> PinArticle(string language, int pageId, string title, Uri uri, Uri imageUri)
    {
      var tileId = string.Format("{0}-{1}", language, pageId);

      if (SecondaryTile.Exists(tileId))
        return true;

      var tile = new SecondaryTile(tileId)
      {
        DisplayName = title,
        Arguments = BuildArguments(language, pageId, title, uri)
      };

      tile.VisualElements.ShowNameOnSquare150x150Logo = true;
      tile.VisualElements.ShowNameOnSquare310x310Logo = true;
      tile.VisualElements.ShowNameOnWide310x150Logo = true;

      tile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.png");
      tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/Square310x310Logo.png");
      tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");

      var created = await tile.RequestCreateAsync();
      if (!created)
        return false;

      var content = CreateTileContent(title, imageUri);
      var notification = new TileNotification(content);

      var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);
      updater.Update(notification);

      return true;
    }

    private static string BuildArguments(string language, int pageId, string title, Uri uri)
    {
      var article = new ArticleHead { PageId = pageId, Title = title, Language = language, Uri = uri };

      return JsonConvert.SerializeObject(article);
    }

    private static XmlDocument CreateTileContent(string title, Uri imageUri)
    {
      var image = imageUri != null ? $"<image src='{imageUri}' placement='background' hint-overlay='50' />" : null;
      var text = $"<text hint-style='caption' hint-wrap='true'>{title}</text>";

      var xml = $@"
      <tile version='3'>
          <visual branding='logo' displayName='Wikini'>
              <binding template='TileSmall'>
                  {image}
                  {text}
              </binding>

              <binding template='TileMedium'>
                  {image}
                  {text}
              </binding>
 
              <binding template='TileWide'>
                  {image}
                  {text}
              </binding>

              <binding template='TileLarge'>
                  {image}
                  {text}
              </binding>
          </visual>
      </tile>";

      var content = new XmlDocument();
      content.LoadXml(xml);

      return content;
    }

    public static ArticleHead ParseArguments(string arguments)
    {
      return JsonConvert.DeserializeObject<ArticleHead>(arguments);
    }
  }
}