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

      var sectionsCollapsedString = settings.SectionsCollapsed ? "true" : "false";

      var html = $@"
        <!DOCTYPE html>
        <html class=""client-nojs"" lang=""{language}"" dir=""{direction}"">
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
        <script async="""" src=""/w/load.php?lang={language}&amp;modules=startup&amp;only=scripts&amp;raw=1&amp;skin=minerva&amp;target=mobile""></script>
        <script src=""ms-appx-web:///Assets/Article/wikini.min.js"" type=""text/javascript""></script>
        <meta name=""ResourceLoaderDynamicStyles"" content=""""/>
        <meta name=""viewport"" content=""initial-scale=1.0, user-scalable=no, width=device-width""/>
        <style>{styles}</style>
        </head>
        <body class=""mediawiki {direction} sitedir-{direction} mw-hide-empty-elt ns-0 ns-subject stable skin-minerva action-view feature-footer-v2"" onload=""registerEventListeners();"" style=""margin-top: {header}px"">
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

    private static string GetArticleStyles(bool darkMode, int fontSize)
    {
      var scaledFontSize = GetScaledFontSize(fontSize);
      var color = App.Current.InDarkMode() ? "#61B7B9" : "#3D918E";

      var styles = @"
        body {font-size: " + scaledFontSize + @"px;}
        body {-ms-overflow-style: none; margin-right:10px;}

        a, a:visited {color:" + color + @" !important;}

        mark { background: #FAFA37; color: black; }
        mark.current {
          background: orange;
        }

        .content .in-block { display:block; }
        .content table.infobox > caption { display: inline; }";

      if (darkMode)
      {
        styles += @"
          html, body {background-color:#121212 !important;}
          .mw-body, #mw-mf-page-center, .feature-footer-v2, .feature-footer-v2 #mw-mf-page-center {background-color:inherit !important;}

          body {color:#E0E0E0 !important;}

          .mw-parser-output .main-box {background-color:#121212 !important;}
          .mw-parser-output .main-top, .mw-parser-output .main-page-body>div:last-of-type {background-color:#333 !important;}
          .mw-parser-output .main-top-left {background-image: linear-gradient(to right,#333 0%,#333 70%,rgba(248,249,250,0)100%) !important;}

          .content .section-heading {border-bottom-color:#333 !important;}
          .content figcaption, .content .thumbcaption {color:#A0A0A0 !important;}
          .content table.infobox {color:#E0E0E0 !important;background-color:#1a1a1a !important;}
          .content table.infobox th[colspan=""2""] {background-color:#333 !important;}
          .content table.infobox th, .content table.infobox td {border-color:#333 !important;}

          li.gallerybox div.thumb, .mbox-small {background-color:#1a1a1a !important;border-color:#333 !important;}
          img.thumbimage {background:#333;padding:5px;}

          .content table.wikitable {border-color:#333 !important;}
          .content table.wikitable > tr > th, .content table.wikitable > * > tr > th {background-color:#333 !important;}
          .content table.wikitable > tr > th, .content table.wikitable > tr > td, .content table.wikitable > * > tr > th, .content table.wikitable > * > tr > td {border-color:#333 !important;}

          .hatnote {background:#1a1a1a !important;}
          .drawer.references-drawer, .mw-graph {background:#333 !important;}
          .drawer-container__mask {background: rgba(255,255,255,0.25);}

          table > * > tr.hintergrundfarbe1 > th, table > * > tr > th.hintergrundfarbe1, table.hintergrundfarbe1, .hintergrundfarbe1 {background-color:#333 !important;}
          table > * > tr.hintergrundfarbe2 > th, table > * > tr > th.hintergrundfarbe2, table.hintergrundfarbe2, .hintergrundfarbe2 {background-color:#000 !important;}
          table > * > tr.hintergrundfarbe5 > th, table > * > tr > th.hintergrundfarbe5, table.hintergrundfarbe5, .hintergrundfarbe5 {background-color:#1a1a1a !important;}

          .mwe-math-fallback-image-inline {filter:invert(0.6);}

          .mw-ui-icon-mf-expand:before {
            background-image: linear-gradient(transparent,transparent),url('data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 fill=%22#bbb%22 width=%2220%22 height=%2220%22 viewBox=%220 0 20 20%22%3E %3Ctitle%3E expand %3C/title%3E %3Cpath d=%22M17.5 4.75l-7.5 7.5-7.5-7.5L1 6.25l9 9 9-9z%22/%3E %3C/svg%3E') !important;
          }";
      }

      return styles;
    }

    private static int GetScaledFontSize(int fontSize)
    {
      const int baseFontSize = 14;

      var scaleFactor = UiSettings.TextScaleFactor * 100;
      var fontFactor = fontSize * 100 / baseFontSize;

      var scaledFontSize = Math.Floor(baseFontSize * (fontFactor / scaleFactor));
      return Convert.ToInt32(scaledFontSize);
    }
  }
}