using System;
using Windows.UI.ViewManagement;

namespace WikipediaApp
{
  public static class WikipediaHtmlBuilder
  {
    private static readonly UISettings UiSettings = new UISettings();

    public static string BuildArticle(string title, string description, string content, string language, string direction, int header)
    {
      var app = App.Current;
      var settings = Settings.Current;

      var darkMode = app.InDarkMode();
      var fontSize = settings.FontSize;
      var styles = GetArticleStyles(darkMode, fontSize);

      var themeClass = GetThemeClass();
      var bodyClasses = themeClass + " " + GetTypefaceClass();
      var sectionsCollapsedString = settings.SectionsCollapsed ? "true" : "false";

      var html = $@"
        <!DOCTYPE html>
        <html class=""client-nojs {themeClass}"" lang=""{language}"" dir=""{direction}"">
        <head>
        <base href=""https://{language}.m.wikipedia.org"" />
        <meta charset=""UTF-8""/>
        <title>{title} - Wikipedia</title>

        <script>document.documentElement.className=""client-js"";RLCONF={{""wgMFCollapseSectionsByDefault"":{sectionsCollapsedString},""wgCanonicalNamespace"":"""",""wgCanonicalSpecialPageName"":!1,""wgNamespaceNumber"":0,""wgPageName"":""{title}"",""wgTitle"":""{title}"",""wgIsArticle"":!0,""wgIsRedirect"":!1,""wgAction"":""view"",""wgUserName"":null,""wgUserGroups"":[""*""],""wgBreakFrames"":!1,""wgPageContentLanguage"":""{language}"",""wgPageContentModel"":""wikitext"",""wgSeparatorTransformTable"":["""",""""],""wgDigitTransformTable"":["""",""""],""wgDefaultDateFormat"":""dmy"",""wgMonthNames"":["""",""January"",""February"",""March"",""April"",""May"",""June"",""July"",""August"",""September"",""October"",""November"",""December""],""wgMonthNamesShort"":["""",""Jan"",""Feb"",""Mar"",""Apr"",""May"",""Jun"",""Jul"",""Aug"",""Sep"",""Oct"",""Nov"",""Dec""],""wgRelevantPageName"":""{title}"",""wgRelevantArticleId"":645042,""wgCSPNonce"":!1,""wgIsProbablyEditable"":!1,""wgRelevantPageIsProbablyEditable"":!1,""wgRestrictionEdit"":[
        ""autoconfirmed""],""wgRestrictionMove"":[""sysop""],""wgMediaViewerOnClick"":!0,""wgMediaViewerEnabledByDefault"":!0,""wgPopupsReferencePreviews"":!1,""wgPopupsConflictsWithNavPopupGadget"":!1,""wgVisualEditor"":{{""pageLanguageCode"":""{language}"",""pageLanguageDir"":""{direction}"",""pageVariantFallbacks"":""{language}""}},""wgMFMode"":""stable"",""wgMFAmc"":!1,""wgMFAmcOutreachActive"":!1,""wgMFAmcOutreachUserEligible"":!1,""wgMFLazyLoadImages"":!0,""wgMFDisplayWikibaseDescriptions"":{{""search"":!0,""nearby"":!0,""watchlist"":!0,""tagline"":!1}},""wgMFDefaultEditor"":""source"",""wgMFIsPageContentModelEditable"":!0,""wgWMESchemaEditAttemptStepOversample"":!1,""wgULSCurrentAutonym"":""English"",""wgNoticeProject"":""wikipedia"",""wgWikibaseItemId"":""Q60"",""wgCentralAuthMobileDomain"":!0,""wgEditSubmitButtonLabelPublish"":!0,""wgMinervaPermissions"":{{""watch"":!0,""talk"":!1}},""wgMinervaFeatures"":{{""beta"":!1,""mobileOptionsLink"":!0,""categories"":!1,""backToTop"":!1,""shareButton"":!1,""pageIssues"":!0,""talkAtTop"":!1,""historyInPageActions"":!1,""overflowSubmenu"":!1,""tabsOnSpecials"":!1,""personalMenu"":!1,""mainMenuExpanded"":!1}},""wgMinervaDownloadNamespaces"":[0]}};RLSTATE={{""user.styles"":""ready"",""user"":""ready"",""user.options"":""ready"",""user.tokens"":""loading"",""ext.kartographer.style"":
        ""ready"",""ext.cite.styles"":""ready"",""ext.graph.styles"":""ready"",""skins.minerva.base.styles"":""ready"",""skins.minerva.content.styles"":""ready"",""skins.minerva.content.styles.images"":""ready"",""mediawiki.hlist"":""ready"",""mediawiki.ui.icon"":""ready"",""mediawiki.ui.button"":""ready"",""skins.minerva.icons.wikimedia"":""ready"",""skins.minerva.icons.images"":""ready"",""mobile.init.styles"":""ready""}};RLPAGEMODULES=[""ext.kartographer.staticframe"",""ext.graph.loader"",""mediawiki.page.startup"",""skins.minerva.options"",""skins.minerva.scripts"",""ext.gadget.switcher"",""ext.centralauth.centralautologin"",""ext.visualEditor.targetLoader"",""mobile.site"",""mobile.init"",""ext.relatedArticles.readMore.bootstrap"",""ext.eventLogging"",""ext.wikimediaEvents"",""ext.navigationTiming"",""mw.externalguidance.init"",""ext.quicksurveys.init"",""ext.centralNotice.geoIP"",""ext.centralNotice.startUp""];</script>
        <script>(RLQ=window.RLQ||[]).push(function(){{mw.loader.implement(""user.tokens@tffin"",function($,jQuery,require,module){{/*@nomin*/mw.user.tokens.set({{""patrolToken"":""+\\"",""watchToken"":""+\\"",""csrfToken"":""+\\""}});
        }});}});</script>

        <link rel=""stylesheet"" href=""/w/load.php?lang={language}&amp;modules=ext.cite.styles%7Cext.graph.styles%7Cext.kartographer.style%7Cmediawiki.hlist%7Cmediawiki.ui.button%2Cicon%7Cmobile.init.styles%7Cskins.minerva.base.styles%7Cskins.minerva.content.styles%7Cskins.minerva.content.styles.images%7Cskins.minerva.icons.images%2Cwikimedia&amp;only=styles&amp;skin=minerva""/>
        <link rel=""stylesheet"" href=""ms-appx-web:///Assets/Article/wikini.min.css""/>
        <script async="""" src=""/w/load.php?lang={language}&amp;modules=startup&amp;only=scripts&amp;raw=1&amp;skin=minerva&amp;target=mobile""></script>
        <script src=""ms-appx-web:///Assets/Article/wikini.min.js"" type=""text/javascript""></script>
        <meta name=""ResourceLoaderDynamicStyles"" content=""""/>
        <meta name=""viewport"" content=""initial-scale=1.0, user-scalable=no, width=device-width""/>
        <style>{styles}</style>
        </head>
        <body class=""{bodyClasses} mediawiki {direction} sitedir-{direction} mw-hide-empty-elt ns-0 ns-subject stable skin-minerva action-view feature-footer-v2"" onload=""registerEventListeners();"" style=""margin-top: {header}px"">
        <div id=""mw-mf-viewport"">
	        <div id=""mw-mf-page-center"">
		        <div id=""content"" class=""mw-body"">
              <div class=""pre-content heading-holder"">
                <h1 id=""section_0"">{title}</h1>
                <span style=""font-size:0.8125em;line-height:1.5;color:gray"">{description}</span>
                <hr style=""margin-bottom:20px;width:100px;height:1px;text-align:left;border:none;background:gray"">
              </div><div id=""bodyContent"" class=""content""><div id=""mw-content-text"" lang=""{language}"" dir=""{direction}"" class=""mw-content-{direction}""><script>function mfTempOpenSection(id){{var block=document.getElementById(""mf-section-""+id);block.className+="" open-block"";block.previousSibling.className+="" open-block"";}}</script>
        {content}
        </div></div></div></div></div></div>
        </body>
        </html>
        ";

      return html;
    }

    public static string BuildTypefaceUpdateJS()
    {
      var classNames = GetThemeClass() + " " + GetTypefaceClass();

      return $"document.body.className='{classNames}';";
    }

    public static string BuildFontSizeUpdateJS()
    {
      var fontSize = GetScaledFontSize(Settings.Current.FontSize);

      return $"document.documentElement.style.setProperty('--font-size', '{fontSize}px');";
    }

    private static string GetArticleStyles(bool darkMode, int fontSize)
    {
      var scaledFontSize = GetScaledFontSize(fontSize);
      var color = darkMode ? "#61B7B9" : "#3D918E";

      var styles = $@"
        :root {{
          --font-size: {scaledFontSize}px;
          --color: {color};
        }}
      ";

      return styles;
    }

    private static int GetScaledFontSize(int fontSize)
    {
      const int baseFontSize = 14;

      var scaleFactor = UiSettings.TextScaleFactor * 100;
      var fontFactor = fontSize * 100 / baseFontSize;

      var scaledFontSize = Math.Round(baseFontSize * (fontFactor / scaleFactor));
      return Convert.ToInt32(scaledFontSize);
    }

    private static string GetThemeClass()
    {
      var darkMode = App.Current.InDarkMode();

      return darkMode ? "theme-dark" : "theme-light";
    }

    private static string GetTypefaceClass()
    {
      var typeface = Settings.Current.Typeface;

      return typeface == Typeface.Serif ? "serif" : "sans-serif";
    }
  }
}