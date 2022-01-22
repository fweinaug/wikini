using System;
using Windows.Globalization;
using Windows.Storage;

namespace WikipediaApp
{
  public class AppSettings : IAppSettings
  {
    public string Language
    {
      get
      {
        var language = ApplicationLanguages.PrimaryLanguageOverride;
        if (!string.IsNullOrEmpty(language))
          return language;


        if (ApplicationLanguages.Languages.Count > 0)
          return ApplicationLanguages.Languages[0].Split('-')[0];

        return "en";
      }
    }

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