using System;
using Windows.UI.ViewManagement;

namespace WikipediaApp
{
  public static class WikipediaHtmlBuilder
  {
    private static readonly UISettings UiSettings = new UISettings();

    public static string BuildArticle(string title, string content, string language)
    {
      var app = App.Current;
      var settings = app.Settings;

      var darkMode = app.InDarkMode();
      var fontSize = settings.FontSize;
      var styles = GetArticleStyles(darkMode, fontSize);

      var sectionsCollapsedString = settings.SectionsCollapsed ? "true" : "false";

      var html = $@"
        <!DOCTYPE html>
        <html class=""client-nojs"" lang=""{language}"" dir=""ltr"">
        <head>
        <base href=""https://{language}.m.wikipedia.org"" />
        <meta charset=""UTF-8""/>
        <title>{title} - Wikipedia</title>

        <script>document.documentElement.className = document.documentElement.className.replace( /(^|\s)client-nojs(\s|$)/, ""$1client-js$2"" );</script>
        <script>(window.RLQ=window.RLQ||[]).push(function(){{mw.config.set({{""wgMFCollapseSectionsByDefault"":{sectionsCollapsedString},""wgCanonicalNamespace"":"""",""wgCanonicalSpecialPageName"":false,""wgNamespaceNumber"":0,""wgPageName"":""Borussia_Dortmund"",""wgTitle"":""{title}"",""wgCurRevisionId"":754715987,""wgRevisionId"":754715987,""wgArticleId"":331715,""wgIsArticle"":true,""wgIsRedirect"":false,""wgAction"":""view"",""wgUserName"":null,""wgUserGroups"":[""*""],""wgBreakFrames"":false,""wgPageContentLanguage"":""{language}"",""wgPageContentModel"":""wikitext"",""wgSeparatorTransformTable"":["""",""""],""wgDigitTransformTable"":["""",""""],""wgDefaultDateFormat"":""dmy"",""wgMonthNames"":["""",""January"",""February"",""March"",""April"",""May"",""June"",""July"",""August"",""September"",""October"",""November"",""December""],""wgMonthNamesShort"":["""",""Jan"",""Feb"",""Mar"",""Apr"",""May"",""Jun"",""Jul"",""Aug"",""Sep"",""Oct"",""Nov"",""Dec""],""wgRelevantPageName"":""Borussia_Dortmund"",""wgRelevantArticleId"":331715,""wgRequestId"":""WFmSiQpAADoAAP1pWFsAAABN"",""wgIsProbablyEditable"":true,""wgRestrictionEdit"":[],""wgRestrictionMove"":[],""wgFlaggedRevsParams"":{{""tags"":{{}}}},""wgStableRevisionId"":null,""wgWikiEditorEnabledModules"":{{""toolbar"":true,""dialogs"":true,""preview"":false,""publish"":false}},""wgBetaFeaturesFeatures"":[],""wgMediaViewerOnClick"":true,""wgMediaViewerEnabledByDefault"":true,""wgVisualEditor"":{{""pageLanguageCode"":""{language}"",""pageLanguageDir"":""ltr"",""usePageImages"":true,""usePageDescriptions"":true}},""wgMFMode"":""stable"",""wgMFLazyLoadImages"":true,""wgMFLazyLoadReferences"":false,""wgPreferredVariant"":""{language}"",""wgMFDisplayWikibaseDescriptions"":{{""search"":true,""nearby"":true,""watchlist"":true,""tagline"":false}},""wgRelatedArticles"":null,""wgRelatedArticlesUseCirrusSearch"":true,""wgRelatedArticlesOnlyUseCirrusSearch"":false,""wgULSCurrentAutonym"":""English"",""wgNoticeProject"":""wikipedia"",""wgCentralNoticeCookiesToDelete"":[],""wgCentralNoticeCategoriesUsingLegacy"":[""Fundraising"",""fundraising""],""wgCategoryTreePageCategoryOptions"":""{{\""mode\"":0,\""hideprefix\"":20,\""showcount\"":true,\""namespaces\"":false}}"",""wgWikibaseItemId"":""Q41420"",""wgCentralAuthMobileDomain"":true,""wgVisualEditorToolbarScrollOffset"":0,""wgEditSubmitButtonLabelPublish"":false,""wgMinervaMenuData"":{{""groups"":[[{{""name"":""home"",""components"":[{{""text"":""Home"",""href"":""/wiki/Main_Page"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-home "",""data-event-name"":""home""}}]}},{{""name"":""random"",""components"":[{{""text"":""Random"",""href"":""/wiki/Special:Random/#/random"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-random "",""id"":""randomButton"",""data-event-name"":""random""}}]}},{{""name"":""nearby"",""components"":[{{""text"":""Nearby"",""href"":""/wiki/Special:Nearby"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-nearby nearby"",""data-event-name"":""nearby""}}],""class"":""jsonly""}}],[{{""name"":""auth"",""components"":[{{""text"":""Log in"",""href"":""/w/index.php?title=Special:UserLogin\u0026returnto=Borussia+Dortmund\u0026returntoquery=welcome%3Dyes"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-anonymous "",""data-event-name"":""login""}}],""class"":""jsonly""}}],[{{""name"":""settings"",""components"":[{{""text"":""Settings"",""href"":""/w/index.php?title=Special:MobileOptions\u0026returnto=Borussia+Dortmund"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-settings "",""data-event-name"":""settings""}}]}}]],""sitelinks"":[{{""name"":""about"",""components"":[{{""text"":""About Wikipedia"",""href"":""/wiki/Wikipedia:About"",""class"":""""}}]}},{{""name"":""disclaimers"",""components"":[{{""text"":""Disclaimers"",""href"":""/wiki/Wikipedia:General_disclaimer"",""class"":""""}}]}}]}},""wgMinervaTocEnabled"":false,""wgMFDescription"":null}});mw.loader.state({{""mobile.usermodule.styles"":""ready"",""user.styles"":""ready"",""user"":""ready"",""user.options"":""loading"",""user.tokens"":""loading"",""mediawiki.page.gallery.styles"":""ready"",""skins.minerva.base.reset"":""ready"",""skins.minerva.base.styles"":""ready"",""skins.minerva.content.styles"":""ready"",""skins.minerva.tablet.styles"":""ready"",""mediawiki.ui.icon"":""ready"",""mediawiki.ui.button"":""ready"",""skins.minerva.icons.images"":""ready"",""skins.minerva.footerV2.styles"":""ready"",""mobile.usermodule"":""ready""}});mw.loader.implement(""user.options@0j3lz3q"",function($,jQuery,require,module){{mw.user.options.set({{""variant"":""{language}""}});}});mw.loader.implement(""user.tokens@1dqfd7l"",function ( $, jQuery, require, module ) {{
        mw.user.tokens.set({{""editToken"":""+\\"",""patrolToken"":""+\\"",""watchToken"":""+\\"",""csrfToken"":""+\\""}});/*@nomin*/;

        }});mw.loader.load([""mediawiki.toc"",""mediawiki.page.startup"",""mediawiki.user"",""mediawiki.hidpi"",""skins.minerva.scripts.top"",""skins.minerva.scripts"",""skins.minerva.watchstar"",""skins.minerva.editor"",""skins.minerva.toggling"",""mobile.site"",""ext.gadget.switcher"",""ext.centralauth.centralautologin"",""ext.visualEditor.targetLoader"",""ext.eventLogging.subscriber"",""ext.navigationTiming"",""ext.quicksurveys.init"",""ext.centralNotice.geoIP"",""ext.centralNotice.startUp""]);}});</script>
        <link rel=""stylesheet"" href=""/w/load.php?debug=false&amp;lang={language}&amp;modules=mediawiki.page.gallery.styles%7Cmediawiki.ui.button%2Cicon%7Cskins.minerva.base.reset%2Cstyles%7Cskins.minerva.content.styles%7Cskins.minerva.content.styles.images%7Cskins.minerva.footerV2.styles%7Cskins.minerva.icons.images%7Cskins.minerva.tablet.styles&amp;only=styles&amp;skin=minerva""/>
        <script async="""" src=""/w/load.php?debug=false&amp;lang={language}&amp;modules=startup&amp;only=scripts&amp;skin=minerva&amp;target=mobile""></script>
        <script src=""ms-appx-web:///Assets/Article/search.js"" type=""text/javascript""></script>
        <meta name=""ResourceLoaderDynamicStyles"" content=""""/>
        <meta name=""viewport"" content=""initial-scale=1.0, user-scalable=no, width=device-width""/>
        <style>{styles}</style>
        </head>
        <body class=""mediawiki ltr sitedir-ltr mw-hide-empty-elt ns-0 ns-subject stable skin-minerva action-view feature-footer-v2"">
        <div id=""mw-mf-viewport"">
	        <div id=""mw-mf-page-center"">
		        <div id=""content"" class=""mw-body"">
			        <div class=""pre-content heading-holder""><h1 id=""section_0"">{title}</h1></div><div id=""bodyContent"" class=""content""><div id=""mw-content-text"" lang=""{language}"" dir=""ltr"" class=""mw-content-ltr""><script>function mfTempOpenSection(id){{var block=document.getElementById(""mf-section-""+id);block.className+="" open-block"";block.previousSibling.className+="" open-block"";}}</script>
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

      var styles = @"
        body {font-size: " + scaledFontSize + @"px;}

        a, a:visited {color:#0063B1;}

        .highlight { background: #FAFA37; color: black; }
        .content table.infobox > caption { display: inline; }";

      if (darkMode)
      {
        styles += @"
          html, body {background-color:#000 !important;}
          .mw-body, #mw-mf-page-center, .feature-footer-v2, .feature-footer-v2 #mw-mf-page-center {background-color:inherit !important;}

          body {color:#999 !important;}

          a, a:visited {color:#2372AF !important;}

          .content .section-heading {border-bottom-color:#333 !important;}
          .content table.infobox {background-color:#1a1a1a !important;}
          .content table.infobox th[colspan=""2""] {background-color:#333 !important;}
          .content table.infobox th, .content table.infobox td {border-color:#333 !important;}

          li.gallerybox div.thumb, .mbox-small {background-color:#1a1a1a !important;border-color:#333 !important;}
          img.thumbimage {background:#333;padding:5px;}

          .content table.wikitable {border-color:#333 !important;}
          .content table.wikitable > tr > th, .content table.wikitable > * > tr > th {background-color:#333 !important;}
          .content table.wikitable > tr > th, .content table.wikitable > tr > td, .content table.wikitable > * > tr > th, .content table.wikitable > * > tr > td {border-color:#333 !important;}

          .hatnote {background:#1a1a1a !important;}
          .mw-graph {background:#333 !important;}

          table > * > tr.hintergrundfarbe2 > th, table > * > tr > th.hintergrundfarbe2, table.hintergrundfarbe2, .hintergrundfarbe2 {background-color:#000 !important;}
          table > * > tr.hintergrundfarbe5 > th, table > * > tr > th.hintergrundfarbe5, table.hintergrundfarbe5, .hintergrundfarbe5 {background-color:#1a1a1a !important;}

          .mwe-math-fallback-image-inline {filter:invert(0.6);}

          .mw-ui-icon-arrow.indicator:before, .mw-ui-icon-mf-arrow:before {
              background-image: linear-gradient(transparent,transparent),url('data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 fill=%22#bbb%22 width=%2224%22 height=%2224%22 viewBox=%220 -407 24 24%22%3E%3Cpath d=%22M21.348-401.268q.94 0 1.61.668l.92.922-11.858 11.86-11.822-11.842.922-.94q.65-.686 1.59-.686.94 0 1.61.668l7.718 7.7 7.7-7.682q.67-.668 1.61-.668z%22/%3E%3C/svg%3E');
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