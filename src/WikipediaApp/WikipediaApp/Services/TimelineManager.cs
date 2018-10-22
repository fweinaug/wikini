using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.Foundation;
using Windows.UI.Shell;

namespace WikipediaApp
{
  public static class TimelineManager
  {
    private static UserActivitySession currentSession = null;

    public static async Task<bool> AddArticle(string language, int pageId, string title, Uri uri, Uri imageUri)
    {
      var channel = UserActivityChannel.GetDefault();
      var userActivity = await channel.GetOrCreateUserActivityAsync("Article-" + language + pageId + "_" + DateTime.Today.Ticks);

      var json = BuildAdaptiveCardJson(title, "Wikipedia", imageUri);

      userActivity.VisualElements.DisplayText = title;
      userActivity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(json);

      userActivity.ActivationUri = new Uri($"wikini://article?language={language}&pageId={pageId}");
      userActivity.FallbackUri = uri;

      await userActivity.SaveAsync();

      currentSession?.Dispose();
      currentSession = userActivity.CreateSession();

      return true;
    }

    private static string BuildAdaptiveCardJson(string title, string description, Uri imageUri)
    {
      return $@"{{
        ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
        ""type"": ""AdaptiveCard"",
        ""version"": ""1.0"",
        ""backgroundImage"": ""{imageUri}"",
        ""body"": [
          {{
            ""type"": ""Container"",
            ""seperation"": ""None"",
            ""items"": [
              {{
                ""type"": ""TextBlock"",
                ""text"": ""{title}"",
                ""weight"": ""bolder"",
                ""size"": ""large"",
                ""wrap"": true,
                ""maxLines"": 3
              }},
              {{
                ""type"": ""TextBlock"",
                ""text"": ""{description}"",
                ""size"": ""default"",
                ""wrap"": true,
                ""maxLines"": 3,
                ""isSubtle"": true
              }}
            ]
          }}
        ]
      }}";
    }

    public static ArticleHead ParseArticle(Uri uri)
    {
      var query = uri.Query;

      var decoder = new WwwFormUrlDecoder(query);
      var language = decoder.GetFirstValueByName("language");
      var pageId = Convert.ToInt32(decoder.GetFirstValueByName("pageId"));

      return new ArticleHead { Language = language, PageId = pageId };
    }
  }
}