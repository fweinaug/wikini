namespace WikipediaApp
{
  public static class WikipediaHtmlBuilder
  {
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
        <link rel=""stylesheet"" href=""/w/load.php?debug=false&amp;lang={language}&amp;modules=mediawiki.page.gallery.styles%7Cmediawiki.ui.button%2Cicon%7Cskins.minerva.base.reset%2Cstyles%7Cskins.minerva.content.styles%7Cskins.minerva.footerV2.styles%7Cskins.minerva.icons.images%7Cskins.minerva.tablet.styles&amp;only=styles&amp;skin=minerva""/>
        <script async="""" src=""/w/load.php?debug=false&amp;lang={language}&amp;modules=startup&amp;only=scripts&amp;skin=minerva&amp;target=mobile""></script>
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
      var styles = @"
body { font-size: " + fontSize + @"px; }

        a, a:visited {color:#0063B1;}";

      if (darkMode)
      {
        styles += @"
          html, body {background-color:#000 !important;}
          .mw-body, #mw-mf-page-center, .feature-footer-v2, .feature-footer-v2 #mw-mf-page-center {background-color:inherit !important;}

          body {color:#999 !important;}

          .content .section-heading {border-bottom-color:#333 !important;}
          .content table.infobox {background-color:#1a1a1a !important;}
          .content table.infobox th[colspan=""2""] {background-color:#333 !important;}
          .content table.infobox th, .content table.infobox td {border-color:#333 !important;}

          li.gallerybox div.thumb, .mbox-small {background-color:#1a1a1a !important;border-color:#333 !important;}

          .content table.wikitable {border-color:#333 !important;}
          .content table.wikitable > tr > th, .content table.wikitable > * > tr > th {background-color:#333 !important;}
          .content table.wikitable > tr > th, .content table.wikitable > tr > td, .content table.wikitable > * > tr > th, .content table.wikitable > * > tr > td {border-color:#333 !important;}

          table > * > tr.hintergrundfarbe2 > th, table > * > tr > th.hintergrundfarbe2, table.hintergrundfarbe2, .hintergrundfarbe2 {background-color:#000 !important;}
          table > * > tr.hintergrundfarbe5 > th, table > * > tr > th.hintergrundfarbe5, table.hintergrundfarbe5, .hintergrundfarbe5 {background-color:#1a1a1a !important;}

          .mwe-math-fallback-image-inline {filter:invert(0.6);}

          .mw-ui-icon-arrow.indicator::before {
              background-image: linear-gradient(transparent, transparent), url('data:image/svg+xml,%3C%3Fxml%20version%3D%221.0%22%3F%3E%0A%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20viewBox%3D%220%20-407%2024%2024%22%3E%3Cg%20fill%3D%22%23BBB%22%3E%3Cg%20xmlns%3Adefault%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20id%3D%22g4%22%3E%3Cpath%20d%3D%22M21.348-401.268q.94%200%201.61.668l.92.922-11.858%2011.86L.198-399.66l.922-.94q.65-.686%201.59-.686.94%200%201.61.668l7.718%207.7%207.7-7.682q.67-.668%201.61-.668z%22%20id%3D%22path6%22%2F%3E%3C%2Fg%3E%3C%2Fg%3E%3C%2Fsvg%3E%0A');
          }";
      }

      return styles;
    }
  }
}