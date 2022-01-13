using System;
using Windows.Storage;

namespace WikipediaApp
{
  public class AppSettings : IAppSettings
  {
    public bool DarkMode => App.Current.InDarkMode();

    public void WriteLastArticle(ArticleHead value)
    {
      var data = value != null ? value.PageId + "|" + value.Language + "|" + value.Title : null;

      ApplicationData.Current.LocalSettings.Values["LastArticle"] = data;
    }

    public ArticleHead ReadLastArticle()
    {
      var data = ApplicationData.Current.LocalSettings.Values["LastArticle"] as string;
      if (string.IsNullOrEmpty(data))
        return null;

      var parts = data.Split('|');
      if (parts.Length < 3)
        return null;

      return new ArticleHead
      {
        PageId = parts[0].Length > 0 ? Convert.ToInt32(parts[0]) : (int?)null,
        Language = parts[1],
        Title = parts[2]
      };
    }
  }
}