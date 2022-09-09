namespace WikipediaApp
{
  public class WikipediaHtmlBuilder : IWikipediaContentBuilder
  {
    private readonly IAppSettings appSettings;
    private readonly IUserSettings userSettings;
    private readonly ISystemSettingProvider systemSettingProvider;

    public WikipediaHtmlBuilder(IAppSettings appSettings, IUserSettings userSettings, ISystemSettingProvider systemSettingProvider)
    {
      this.appSettings = appSettings;
      this.userSettings = userSettings;
      this.systemSettingProvider = systemSettingProvider;
    }

    public string GetContent(Article article, int headerHeight)
    {
      var fontSize = userSettings.Get<int>(UserSettingsKey.ArticleFontSize);
      var typeface = userSettings.Get<Typeface>(UserSettingsKey.ArticleTypeface);

      var styles = GetRootStyle(fontSize, systemSettingProvider.TextScaleFactor, headerHeight);

      var themeClass = GetThemeClass(appSettings.DarkMode);
      var bodyClasses = themeClass + " " + GetTypefaceClass(typeface);

      var html = article.Html;
      html = html.Insert(html.IndexOf(" class=\"", html.IndexOf("<html ")) + 8, themeClass + " ");
      html = html.Insert(html.IndexOf(" class=\"", html.IndexOf("<body ")) + 8, bodyClasses + " ");
      html = html.Insert(html.IndexOf("<body ") + 6, "onload=\"registerEventListeners();\" ");
      html = html.Insert(html.IndexOf("</head>"), "<link rel=\"stylesheet\" href=\"ms-appx-web:///Assets/Article/article.min.css\"/>");
      html = html.Insert(html.IndexOf("</head>"), "<script src=\"ms-appx-web:///Assets/Article/article.min.js\" type=\"text/javascript\"></script>");
      html = html.Insert(html.IndexOf("</head>"), $"<style>{styles}</style>");

      return html;
    }

    private static string GetRootStyle(int fontSize, double textScaleFactor, int headerHeight)
    {
      var styles = $@"
        :root {{
          --font-size: {fontSize}px;
          --base-font-size: 14;
          --text-scale-factor: {textScaleFactor};
          --header-top: {headerHeight}px;
        }}
      ";

      return styles;
    }

    private static string GetThemeClass(bool darkMode)
    {
      return darkMode ? "theme-dark" : "theme-light";
    }

    private static string GetTypefaceClass(Typeface typeface)
    {
      return typeface == Typeface.Serif ? "serif" : "sans-serif";
    }
  }
}