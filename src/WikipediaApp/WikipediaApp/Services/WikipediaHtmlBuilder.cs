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

    public string GetContent(Article article, int header)
    {
      var fontSize = userSettings.Get<int>(UserSettingsKey.ArticleFontSize);
      var typeface = userSettings.Get<Typeface>(UserSettingsKey.ArticleTypeface);
      var sectionsCollapsed = userSettings.Get<bool>(UserSettingsKey.SectionsCollapsed);

      var styles = GetRootStyle(fontSize, systemSettingProvider.TextScaleFactor);

      var themeClass = GetThemeClass(appSettings.DarkMode);
      var bodyClasses = themeClass + " " + GetTypefaceClass(typeface);
      var sectionsCollapsedString = sectionsCollapsed ? "true" : "false";

      var html = $@"
        <!DOCTYPE html>
        <html class=""client-nojs {themeClass}"" lang=""{article.Language}"" dir=""{article.TextDirection}"">
        <head>
        <base href=""https://{article.Language}.m.wikipedia.org"" />
        <meta charset=""UTF-8""/>
        <title>{article.Title} - Wikipedia</title>

        <script>document.documentElement.className=""client-js"";RLCONF={{""wgMFCollapseSectionsByDefault"":{sectionsCollapsedString},""wgCanonicalNamespace"":"""",""wgCanonicalSpecialPageName"":!1,""wgNamespaceNumber"":0,""wgPageName"":""{article.Title}"",""wgTitle"":""{article.Title}"",""wgIsArticle"":!0,""wgIsRedirect"":!1,""wgAction"":""view"",""wgUserName"":null,""wgUserGroups"":[""*""],""wgBreakFrames"":!1,""wgPageContentLanguage"":""{article.Language}"",""wgPageContentModel"":""wikitext"",""wgSeparatorTransformTable"":["""",""""],""wgDigitTransformTable"":["""",""""],""wgDefaultDateFormat"":""dmy"",""wgMonthNames"":["""",""January"",""February"",""March"",""April"",""May"",""June"",""July"",""August"",""September"",""October"",""November"",""December""],""wgMonthNamesShort"":["""",""Jan"",""Feb"",""Mar"",""Apr"",""May"",""Jun"",""Jul"",""Aug"",""Sep"",""Oct"",""Nov"",""Dec""],""wgRelevantPageName"":""{article.Title}"",""wgRelevantArticleId"":645042,""wgCSPNonce"":!1,""wgIsProbablyEditable"":!1,""wgRelevantPageIsProbablyEditable"":!1,""wgRestrictionEdit"":[
        ""autoconfirmed""],""wgRestrictionMove"":[""sysop""],""wgMediaViewerOnClick"":!0,""wgMediaViewerEnabledByDefault"":!0,""wgPopupsReferencePreviews"":!1,""wgPopupsConflictsWithNavPopupGadget"":!1,""wgVisualEditor"":{{""pageLanguageCode"":""{article.Language}"",""pageLanguageDir"":""{article.TextDirection}"",""pageVariantFallbacks"":""{article.Language}""}},""wgMFMode"":""stable"",""wgMFAmc"":!1,""wgMFAmcOutreachActive"":!1,""wgMFAmcOutreachUserEligible"":!1,""wgMFLazyLoadImages"":!0,""wgMFDisplayWikibaseDescriptions"":{{""search"":!0,""nearby"":!0,""watchlist"":!0,""tagline"":!1}},""wgMFDefaultEditor"":""source"",""wgMFIsPageContentModelEditable"":!0,""wgWMESchemaEditAttemptStepOversample"":!1,""wgULSCurrentAutonym"":""English"",""wgNoticeProject"":""wikipedia"",""wgWikibaseItemId"":""Q60"",""wgCentralAuthMobileDomain"":!0,""wgEditSubmitButtonLabelPublish"":!0,""wgMinervaPermissions"":{{""watch"":!0,""talk"":!1}},""wgMinervaFeatures"":{{""beta"":!1,""mobileOptionsLink"":!0,""categories"":!1,""backToTop"":!1,""shareButton"":!1,""pageIssues"":!0,""talkAtTop"":!1,""historyInPageActions"":!1,""overflowSubmenu"":!1,""tabsOnSpecials"":!1,""personalMenu"":!1,""mainMenuExpanded"":!1}},""wgMinervaDownloadNamespaces"":[0]}};RLSTATE={{""user.styles"":""ready"",""user"":""ready"",""user.options"":""ready"",""user.tokens"":""loading"",""ext.kartographer.style"":
        ""ready"",""ext.cite.styles"":""ready"",""ext.graph.styles"":""ready"",""skins.minerva.base.styles"":""ready"",""skins.minerva.content.styles"":""ready"",""skins.minerva.content.styles.images"":""ready"",""mediawiki.hlist"":""ready"",""mediawiki.ui.icon"":""ready"",""mediawiki.ui.button"":""ready"",""skins.minerva.icons.wikimedia"":""ready"",""skins.minerva.icons.images"":""ready"",""mobile.init.styles"":""ready""}};RLPAGEMODULES=[""ext.kartographer.staticframe"",""ext.graph.loader"",""mediawiki.page.startup"",""skins.minerva.options"",""skins.minerva.scripts"",""ext.gadget.switcher"",""ext.centralauth.centralautologin"",""ext.visualEditor.targetLoader"",""mobile.site"",""mobile.init"",""ext.relatedArticles.readMore.bootstrap"",""ext.eventLogging"",""ext.wikimediaEvents"",""ext.navigationTiming"",""mw.externalguidance.init"",""ext.quicksurveys.init"",""ext.centralNotice.geoIP"",""ext.centralNotice.startUp""];</script>
        <script>(RLQ=window.RLQ||[]).push(function(){{mw.loader.implement(""user.tokens@tffin"",function($,jQuery,require,module){{/*@nomin*/mw.user.tokens.set({{""patrolToken"":""+\\"",""watchToken"":""+\\"",""csrfToken"":""+\\""}});
        }});}});</script>

        <link rel=""stylesheet"" href=""/w/load.php?lang={article.Language}&amp;modules=ext.cite.styles%7Cext.graph.styles%7Cext.kartographer.style%7Cmediawiki.hlist%7Cmediawiki.ui.button%2Cicon%7Cmobile.init.styles%7Cskins.minerva.base.styles%7Cskins.minerva.content.styles%7Cskins.minerva.content.styles.images%7Cskins.minerva.icons.images%2Cwikimedia&amp;only=styles&amp;skin=minerva""/>
        <link rel=""stylesheet"" href=""ms-appx-web:///Assets/Article/article.min.css""/>
        <script async="""" src=""/w/load.php?lang={article.Language}&amp;modules=startup&amp;only=scripts&amp;raw=1&amp;skin=minerva&amp;target=mobile""></script>
        <script src=""ms-appx-web:///Assets/Article/article.min.js"" type=""text/javascript""></script>
        <meta name=""ResourceLoaderDynamicStyles"" content=""""/>
        <meta name=""viewport"" content=""initial-scale=1.0, user-scalable=no, width=device-width""/>
        <style>{styles}</style>
        </head>
        <body class=""{bodyClasses} mediawiki {article.TextDirection} sitedir-{article.TextDirection} mw-hide-empty-elt ns-0 ns-subject stable skin-minerva action-view feature-footer-v2"" onload=""registerEventListeners();"" style=""margin-top: {header}px"">
        <div id=""mw-mf-viewport"">
	        <div id=""mw-mf-page-center"">
		        <div id=""content"" class=""mw-body"">
              <div class=""pre-content heading-holder"">
                <h1 id=""section_0"">{article.Title}</h1>
                <span style=""font-size:0.8125em;line-height:1.5;color:gray"">{article.Description}</span>
                <hr style=""margin-bottom:20px;width:100px;height:1px;text-align:left;border:none;background:gray"">
              </div><div id=""bodyContent"" class=""content""><div id=""mw-content-text"" lang=""{article.Language}"" dir=""{article.TextDirection}"" class=""mw-content-{article.TextDirection}""><script>function mfTempOpenSection(id){{var block=document.getElementById(""mf-section-""+id);block.className+="" open-block"";block.previousSibling.className+="" open-block"";}}</script>
        {article.Content}
        </div></div></div></div></div></div>
        </body>
        </html>
        ";

      return html;
    }

    private static string GetRootStyle(int fontSize, double textScaleFactor)
    {
      var styles = $@"
        :root {{
          --font-size: {fontSize}px;
          --base-font-size: 14;
          --text-scale-factor: {textScaleFactor};
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