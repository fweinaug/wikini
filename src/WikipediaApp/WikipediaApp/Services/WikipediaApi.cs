using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;
using Microsoft.HockeyApp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WikipediaApp
{
  public class WikipediaQueryApi : WikipediaApi
  {
    public async Task<ArticleHead> GetMainPage(string language)
    {
      try
      {
        const string query = "action=query&meta=siteinfo";

        var result = await QueryAndParse<SiteinfoRoot>(language, query);

        var general = result?.query?.general;
        if (general == null)
          return null;

        var title = result.query.general.mainpage;
        var uri = new Uri(result.query.general.@base);

        return new ArticleHead { Language = language, Title = title, Uri = uri };
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    public async Task<ArticleHead> GetRandomArticle(string language)
    {
      try
      {
        const string query = "action=query&generator=random&grnnamespace=0&grnlimit=1&grnfilterredir=redirects&redirects&prop=info&inprop=url";

        var result = await QueryAndParse<RandomRoot>(language, query);

        var pages = result?.query?.pages;
        if (pages == null || pages.Count == 0)
          return null;

        var page = pages.First();

        var pageId = page.pageid;
        var uri = new Uri(page.fullurl);

        return new ArticleHead { Language = language, Id = pageId, Uri = uri };
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    private class SiteinfoRoot
    {
      public bool batchcomplete { get; set; }
      public SiteinfoQuery query { get; set; }
    }

    private class SiteinfoQuery
    {
      public SiteinfoGeneral general { get; set; }
    }

    private class SiteinfoGeneral
    {
      public string mainpage { get; set; }
      public string @base { get; set; }
    }

    private class RandomRoot
    {
      public bool batchcomplete { get; set; }
      public RandomContinue @continue { get; set; }
      public RandomQuery query { get; set; }
    }

    private class RandomContinue
    {
      public string grncontinue { get; set; }
      public string @continue { get; set; }
    }

    private class RandomQuery
    {
      public List<Page> pages { get; set; }
    }

    private class Page
    {
      public int pageid { get; set; }
      public int ns { get; set; }
      public string title { get; set; }
      public string fullurl { get; set; }
    }
  }

  public class WikipediaParseApi : WikipediaApi
  {
    public async Task<Article> FetchArticle(string language, bool sectionsCollapsed, Uri uri, int? pageId = null, string title = null, Article article = null)
    {
      var query = "action=parse&prop=text|sections|langlinks|headhtml&disableeditsection=&disabletoc=&mobileformat=&";
      if (pageId != null)
        query += "pageid=" + pageId;
      else
        query += "page=" + title;

      var rootObject = await QueryAndParse<ParseRoot>(language, query);
      var parseResult = rootObject?.parse;
      if (parseResult == null)
        return null;

      var parseTitle = parseResult.title;
      var parseContent = parseResult.text;

      var sectionsCollapsedString = sectionsCollapsed ? "true" : "false";

      var content = $@"
<!DOCTYPE html>
<html class=""client-nojs"" lang=""{language}"" dir=""ltr"">
<head>
<base href=""https://{language}.m.wikipedia.org"" />
<meta charset=""UTF-8""/>
<title>{parseTitle} - Wikipedia</title>
<script>document.documentElement.className = document.documentElement.className.replace( /(^|\s)client-nojs(\s|$)/, ""$1client-js$2"" );</script>
<script>(window.RLQ=window.RLQ||[]).push(function(){{mw.config.set({{""wgMFCollapseSectionsByDefault"":{sectionsCollapsedString},""wgCanonicalNamespace"":"""",""wgCanonicalSpecialPageName"":false,""wgNamespaceNumber"":0,""wgPageName"":""Borussia_Dortmund"",""wgTitle"":""{parseTitle}"",""wgCurRevisionId"":754715987,""wgRevisionId"":754715987,""wgArticleId"":331715,""wgIsArticle"":true,""wgIsRedirect"":false,""wgAction"":""view"",""wgUserName"":null,""wgUserGroups"":[""*""],""wgBreakFrames"":false,""wgPageContentLanguage"":""en"",""wgPageContentModel"":""wikitext"",""wgSeparatorTransformTable"":["""",""""],""wgDigitTransformTable"":["""",""""],""wgDefaultDateFormat"":""dmy"",""wgMonthNames"":["""",""January"",""February"",""March"",""April"",""May"",""June"",""July"",""August"",""September"",""October"",""November"",""December""],""wgMonthNamesShort"":["""",""Jan"",""Feb"",""Mar"",""Apr"",""May"",""Jun"",""Jul"",""Aug"",""Sep"",""Oct"",""Nov"",""Dec""],""wgRelevantPageName"":""Borussia_Dortmund"",""wgRelevantArticleId"":331715,""wgRequestId"":""WFmSiQpAADoAAP1pWFsAAABN"",""wgIsProbablyEditable"":true,""wgRestrictionEdit"":[],""wgRestrictionMove"":[],""wgFlaggedRevsParams"":{{""tags"":{{}}}},""wgStableRevisionId"":null,""wgWikiEditorEnabledModules"":{{""toolbar"":true,""dialogs"":true,""preview"":false,""publish"":false}},""wgBetaFeaturesFeatures"":[],""wgMediaViewerOnClick"":true,""wgMediaViewerEnabledByDefault"":true,""wgVisualEditor"":{{""pageLanguageCode"":""en"",""pageLanguageDir"":""ltr"",""usePageImages"":true,""usePageDescriptions"":true}},""wgMFMode"":""stable"",""wgMFLazyLoadImages"":true,""wgMFLazyLoadReferences"":false,""wgPreferredVariant"":""en"",""wgMFDisplayWikibaseDescriptions"":{{""search"":true,""nearby"":true,""watchlist"":true,""tagline"":false}},""wgRelatedArticles"":null,""wgRelatedArticlesUseCirrusSearch"":true,""wgRelatedArticlesOnlyUseCirrusSearch"":false,""wgULSCurrentAutonym"":""English"",""wgNoticeProject"":""wikipedia"",""wgCentralNoticeCookiesToDelete"":[],""wgCentralNoticeCategoriesUsingLegacy"":[""Fundraising"",""fundraising""],""wgCategoryTreePageCategoryOptions"":""{{\""mode\"":0,\""hideprefix\"":20,\""showcount\"":true,\""namespaces\"":false}}"",""wgWikibaseItemId"":""Q41420"",""wgCentralAuthMobileDomain"":true,""wgVisualEditorToolbarScrollOffset"":0,""wgEditSubmitButtonLabelPublish"":false,""wgMinervaMenuData"":{{""groups"":[[{{""name"":""home"",""components"":[{{""text"":""Home"",""href"":""/wiki/Main_Page"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-home "",""data-event-name"":""home""}}]}},{{""name"":""random"",""components"":[{{""text"":""Random"",""href"":""/wiki/Special:Random/#/random"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-random "",""id"":""randomButton"",""data-event-name"":""random""}}]}},{{""name"":""nearby"",""components"":[{{""text"":""Nearby"",""href"":""/wiki/Special:Nearby"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-nearby nearby"",""data-event-name"":""nearby""}}],""class"":""jsonly""}}],[{{""name"":""auth"",""components"":[{{""text"":""Log in"",""href"":""/w/index.php?title=Special:UserLogin\u0026returnto=Borussia+Dortmund\u0026returntoquery=welcome%3Dyes"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-anonymous "",""data-event-name"":""login""}}],""class"":""jsonly""}}],[{{""name"":""settings"",""components"":[{{""text"":""Settings"",""href"":""/w/index.php?title=Special:MobileOptions\u0026returnto=Borussia+Dortmund"",""class"":""mw-ui-icon mw-ui-icon-before mw-ui-icon-mf-settings "",""data-event-name"":""settings""}}]}}]],""sitelinks"":[{{""name"":""about"",""components"":[{{""text"":""About Wikipedia"",""href"":""/wiki/Wikipedia:About"",""class"":""""}}]}},{{""name"":""disclaimers"",""components"":[{{""text"":""Disclaimers"",""href"":""/wiki/Wikipedia:General_disclaimer"",""class"":""""}}]}}]}},""wgMinervaTocEnabled"":true,""wgMFDescription"":null}});mw.loader.state({{""mobile.usermodule.styles"":""ready"",""user.styles"":""ready"",""user"":""ready"",""user.options"":""loading"",""user.tokens"":""loading"",""mediawiki.page.gallery.styles"":""ready"",""skins.minerva.base.reset"":""ready"",""skins.minerva.base.styles"":""ready"",""skins.minerva.content.styles"":""ready"",""skins.minerva.tablet.styles"":""ready"",""mediawiki.ui.icon"":""ready"",""mediawiki.ui.button"":""ready"",""skins.minerva.icons.images"":""ready"",""skins.minerva.footerV2.styles"":""ready"",""mobile.usermodule"":""ready""}});mw.loader.implement(""user.options@0j3lz3q"",function($,jQuery,require,module){{mw.user.options.set({{""variant"":""en""}});}});mw.loader.implement(""user.tokens@1dqfd7l"",function ( $, jQuery, require, module ) {{
mw.user.tokens.set({{""editToken"":""+\\"",""patrolToken"":""+\\"",""watchToken"":""+\\"",""csrfToken"":""+\\""}});/*@nomin*/;

}});mw.loader.load([""mediawiki.toc"",""mediawiki.page.startup"",""mediawiki.user"",""mediawiki.hidpi"",""skins.minerva.scripts.top"",""skins.minerva.scripts"",""skins.minerva.watchstar"",""skins.minerva.editor"",""skins.minerva.toggling"",""mobile.site"",""ext.gadget.switcher"",""ext.centralauth.centralautologin"",""ext.visualEditor.targetLoader"",""ext.eventLogging.subscriber"",""ext.wikimediaEvents"",""ext.navigationTiming"",""ext.quicksurveys.init"",""ext.centralNotice.geoIP"",""ext.centralNotice.startUp""]);}});</script>
<link rel=""stylesheet"" href=""/w/load.php?debug=false&amp;lang=en&amp;modules=mediawiki.page.gallery.styles%7Cmediawiki.ui.button%2Cicon%7Cskins.minerva.base.reset%2Cstyles%7Cskins.minerva.content.styles%7Cskins.minerva.footerV2.styles%7Cskins.minerva.icons.images%7Cskins.minerva.tablet.styles&amp;only=styles&amp;skin=minerva""/>
<script async="""" src=""/w/load.php?debug=false&amp;lang=en&amp;modules=startup&amp;only=scripts&amp;skin=minerva&amp;target=mobile""></script>
<meta name=""ResourceLoaderDynamicStyles"" content=""""/>
<meta name=""generator"" content=""MediaWiki 1.29.0-wmf.6""/>
<meta name=""referrer"" content=""origin-when-cross-origin""/>
<meta name=""viewport"" content=""initial-scale=1.0, user-scalable=yes, minimum-scale=0.25, maximum-scale=5.0, width=device-width""/>
<link rel=""manifest"" href=""/w/api.php?action=webapp-manifest""/>
<link rel=""copyright"" href=""//creativecommons.org/licenses/by-sa/3.0/""/>
</head>
<body class=""mediawiki ltr sitedir-ltr mw-hide-empty-elt ns-0 ns-subject stable skin-minerva action-view feature-footer-v2"">
<div id=""mw-mf-viewport"">
	<div id=""mw-mf-page-center"">
		<div id=""content"" class=""mw-body"">
			<div class=""pre-content heading-holder""><h1 id=""section_0"">{parseTitle}</h1></div><div id=""bodyContent"" class=""content""><div id=""mw-content-text"" lang=""en"" dir=""ltr"" class=""mw-content-ltr""><script>function mfTempOpenSection(id){{var block=document.getElementById(""mf-section-""+id);block.className+="" open-block"";block.previousSibling.className+="" open-block"";}}</script>
{parseContent}
</div></div></div></div></div></div>
</body>
</html>
";

      var sections = new List<ArticleSection>();
      if (parseResult.sections != null && parseResult.sections.Count > 0)
      {
        foreach (var section in parseResult.sections)
        {
          sections.Add(new ArticleSection
          {
            Level = section.toclevel,
            Number = section.number,
            Headline = !string.IsNullOrEmpty(section.line) ? ReplaceHmtl(section.line) : null,
            Anchor = section.anchor
          });
        }
      }

      var languages = new List<ArticleLanguage>();
      if (parseResult.langlinks != null && parseResult.langlinks.Count > 0)
      {
        foreach (var langling in parseResult.langlinks)
        {
          languages.Add(new ArticleLanguage
          {
            Name = string.IsNullOrEmpty(langling.autonym) ? langling.langname : langling.autonym,
            Uri = new Uri(langling.url)
          });
        }
      }

      if (article == null)
        article = new Article();

      article.Language = language;
      article.PageId = parseResult.pageid;
      article.Title = parseTitle;
      article.Content = content;
      article.Uri = uri;
      article.Sections = sections;
      article.Languages = languages;

      return article;
    }

    private class ParseRoot
    {
      public ParseResult parse { get; set; }
    }

    private class ParseResult
    {
      public string title { get; set; }
      public int pageid { get; set; }
      public string text { get; set; }
      public string headhtml { get; set; }
      public List<ParseSection> sections { get; set; }
      public List<ParseLanglink> langlinks { get; set; }
    }

    private class ParseLanglink
    {
      public string lang { get; set; }
      public string url { get; set; }
      public string langname { get; set; }
      public string autonym { get; set; }
    }

    private class ParseSection
    {
      public int toclevel { get; set; }
      public string level { get; set; }
      public string line { get; set; }
      public string number { get; set; }
      public string index { get; set; }
      public string fromtitle { get; set; }
      public int? byteoffset { get; set; }
      public string anchor { get; set; }
    }
  }

  public class WikipediaSearchApi : WikipediaApi
  {
    public async Task<IList<ArticleHead>> Search(string searchTerm, string language)
    {
      var list = new List<ArticleHead>();

      var query = "action=opensearch&search=" + searchTerm + "&namespace=0&redirects=resolve&limit=10";
      var response = await SendRequest(language, query);
      if (string.IsNullOrEmpty(response))
        return list;

      var array = JArray.Parse(response);

      var titles = array[1].Value<JArray>();
      var urls = array[3].Value<JArray>();

      for (var i = 0; i < titles.Count; ++i)
      {
        var article = new ArticleHead
        {
          Title = titles[i].Value<string>(),
          Language = language,
          Uri = new Uri(urls[i].Value<string>())
        };

        list.Add(article);
      }

      return list;
    }
  }

  public abstract class WikipediaApi
  {
    protected async Task<T> QueryAndParse<T>(string language, string query)
    {
      var response = await SendRequest(language, query);
      var result = JsonConvert.DeserializeObject<T>(response);

      return result;
    }

    protected async Task<string> SendRequest(string language, string query)
    {
      var client = new HttpClient();

      var requestUri = new Uri("https://" + language + ".wikipedia.org/w/api.php?format=json&formatversion=2&" + query);
      var response = await client.GetStringAsync(requestUri);

      return response;
    }

    protected static string ReplaceHmtl(string text)
    {
      if (text.Contains("<"))
        text = Regex.Replace(text, "<[^>]*>", string.Empty);

      text = WebUtility.HtmlDecode(text);
      text = text.Replace("\r", string.Empty);
      text = text.Replace("\n", string.Empty);

      return text;
    }
  }
}