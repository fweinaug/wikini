using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Web.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace WikipediaApp
{
  public class WikipediaQueryApi : WikipediaApi
  {
    public async Task<ArticleHead> GetMainPage(string language)
    {
      const string query = "action=query&meta=siteinfo";

      var result = await QueryAndParse<SiteinfoRoot>(language, query);

      var general = result?.query?.general;
      if (general == null)
        return null;

      return await GetArticleInfo(language, null, general.mainpage);
    }

    public async Task<ArticleHead> GetRandomArticle(string language)
    {
      var query = $"action=query&generator=random&grnnamespace=0&grnlimit=1&grnfilterredir=redirects&redirects&prop=info|description|pageimages&inprop=url&pilicense=any&pilimit=1&pithumbsize={ThumbnailSize}";

      var result = await QueryAndParse<PagesRoot>(language, query);

      var pages = result?.query?.pages;
      if (pages == null || pages.Count == 0)
        return null;

      var page = pages.First();

      return MapPage(page);
    }

    public async Task<ArticleHead> GetArticleInfo(string language, int? pageId, string title)
    {
      var query = $"action=query&prop=info|description|pageimages&inprop=url&pilicense=any&pilimit=1&pithumbsize={ThumbnailSize}";
      if (pageId != null)
        query += "&pageids=" + pageId;
      else
        query += "&titles=" + title + "&redirects=";

      var result = await QueryAndParse<PagesRoot>(language, query);

      var pages = result?.query?.pages;
      if (pages == null || pages.Count == 0)
        return null;

      var page = pages.First();

      return MapPage(page);
    }

    private static ArticleHead MapPage(PagesPage page)
    {
      return new ArticleHead
      {
        Language = page.pagelanguage,
        PageId = page.pageid,
        Title = page.title,
        Description = page.description,
        Uri = new Uri(page.fullurl),
        ThumbnailUri = !string.IsNullOrEmpty(page.thumbnail?.source) ? new Uri(page.thumbnail.source) : null
      };
    }

    public async Task<PictureOfTheDay> GetPictureOfTheDay(DateTime date)
    {
      var query = $"https://commons.wikimedia.org/w/api.php?format=json&formatversion=2&action=query&generator=images&titles=Template:Potd/{date:yyyy-MM-dd}&prop=imageinfo|revisions&iiprop=url&iiurlwidth=1000&rvprop=content&rvslots=main";

      var result = await QueryAndParse<ImagesGeneratorRoot>(query);

      var pages = result?.query?.pages;
      if (pages == null || pages.Count == 0)
        return null;

      var page = pages.First();
      if (page.imageinfo == null || page.imageinfo.Count == 0)
        return null;

      var descriptions = ParseDescriptions(page);

      return new PictureOfTheDay
      {
        ImageUri = new Uri(page.imageinfo[0].url),
        ThumbnailUri = new Uri(page.imageinfo[0].thumburl),
        Descriptions = descriptions,
      };
    }

    private static Dictionary<string, string> ParseDescriptions(ImagesGeneratorPage page)
    {
      var descriptions = new Dictionary<string, string>();

      var content = page.revisions[0].slots["main"].content;

      var descriptionRegex = new Regex(@"\|[Dd]escription\s*=\s*(?<value>[\s\S]+?)\s+\|\S", RegexOptions.Singleline);
      var descriptionMatch = descriptionRegex.Match(content);
      if (!descriptionMatch.Success)
        return descriptions;

      var descriptionValue = descriptionMatch.Groups["value"].Value;
      var descriptionGroups = descriptionValue.Replace(@"\n", "\n").Replace("}}{{", "}}\n{{").Split("\n");
      
      foreach (var group in descriptionGroups)
      {
        var groupValue = group.Substring(2, group.Length - 4); // remove curly brackets
        var descriptionOffset = groupValue.IndexOf('|');
        if (descriptionOffset != 2)
          continue;

        var language = groupValue.Substring(0, descriptionOffset);

        var description = groupValue.Substring(descriptionOffset + 1).TrimEnd();
        if (description.StartsWith("1="))
          description = description.Substring(2);

        descriptions.Add(language, RemoveHtml(description));
      }

      return descriptions;
    }

    public async Task<List<ArticleImage>> GetArticleImages(Article article)
    {
      var imagesByFilenameDictionary = await FetchArticleImagesGroupedByFilename(article.PageId.Value, article.Language);

      var images = ParseArticleForImages(article, imagesByFilenameDictionary);
      return images;
    }

    private async Task<Dictionary<string, ArticleImage>> FetchArticleImagesGroupedByFilename(int pageId, string language)
    {
      var dictionary = new Dictionary<string, ArticleImage>();

      var query = "action=query&generator=images&gimlimit=50&prop=imageinfo&iiprop=url&iiurlwidth=300&iiurlheight=300";
      query += "&pageids=" + pageId;

      var query2 = query;

      while (true)
      {
        var result = await QueryAndParse<ImagesGeneratorRoot>(language, query2);

        var pages = result?.query?.pages;
        if (pages == null || pages.Count == 0)
          break;

        foreach (var page in pages)
        {
          if (page.imageinfo == null || page.imageinfo.Count == 0 || string.IsNullOrEmpty(page.imageinfo[0].url))
            continue;

          var imageinfo = page.imageinfo[0];

          var imageUri = new Uri(imageinfo.url.EndsWith(".svg", StringComparison.InvariantCultureIgnoreCase)
            ? imageinfo.thumburl.Replace("/300px-", "/1400px-")
            : imageinfo.url);
          var thumbnailUri = new Uri(imageinfo.thumburl);
          var descriptionUrl = new Uri(imageinfo.descriptionurl);

          var filename = Path.GetFileName(descriptionUrl.AbsolutePath);
          filename = filename.Substring(filename.IndexOf(':') + 1);

          var name = Uri.UnescapeDataString(filename);

          if (!dictionary.ContainsKey(filename))
            dictionary.Add(filename, new ArticleImage { Name = name, ImageUri = imageUri, ThumbnailUri = thumbnailUri });
        }

        if (result.@continue != null)
        {
          query2 = query + "&continue=" + result.@continue.@continue + "&gimcontinue=" + result.@continue.gimcontinue;
        }
        else
          break;
      }

      return dictionary;
    }

    private static readonly string[] ImageFileExtensions = { "jpg", "png", "bmp", "svg", "gif", "tif", "tiff" };

    private List<ArticleImage> ParseArticleForImages(Article article, Dictionary<string, ArticleImage> imagesByFilenameDictionary)
    {
      var list = new List<ArticleImage>();

      var document = new HtmlDocument();
      document.LoadHtml(article.Html);

      var linkNodes = document.DocumentNode.Descendants("a")
        .Where(node => node.GetAttributeValue("class", null) == "image")
        .ToList();

      var foundFilenames = new List<string>();

      foreach (var linkNode in linkNodes)
      {
        var imageNode = linkNode.Descendants("img").SingleOrDefault();
        if (imageNode == null)
          continue;

        var link = linkNode.GetAttributeValue("href", null);
        if (string.IsNullOrEmpty(link))
          continue;

        var filename = link.Substring(link.IndexOf(':') + 1);
        if (!imagesByFilenameDictionary.ContainsKey(filename) || foundFilenames.Contains(filename))
          continue;

        var width = imageNode.GetAttributeValue("width", 0);
        var height = imageNode.GetAttributeValue("height", 0);
        if (width < 100 && height < 100)
          continue;

        var image = imagesByFilenameDictionary[filename];
        image.Description = GetImageDescription(linkNode, imageNode);

        foundFilenames.Add(filename);
        list.Add(image);
      }

      return list;
    }

    private static string GetImageDescription(HtmlNode linkNode, HtmlNode imageNode)
    {
      var description = GetImageDescriptionFromThumbnail(linkNode);
      if (!string.IsNullOrEmpty(description))
        return description;

      description = GetImageDescriptionFromGallery(linkNode);
      if (!string.IsNullOrEmpty(description))
        return description;

      description = GetImageDescriptionFromAltAttribute(imageNode);
      if (!string.IsNullOrEmpty(description))
        return description;

      return null;
    }

    private static string GetImageDescriptionFromAltAttribute(HtmlNode node)
    {
      var alt = node.GetAttributeValue("alt", null);

      if (!string.IsNullOrEmpty(alt))
      {
        var index = alt.LastIndexOf('.');
        if (index > 0 && index + 1 < alt.Length)
        {
          var extension = alt.Substring(index + 1).ToLower();
          if (ImageFileExtensions.Contains(extension))
            alt = alt.Substring(0, index);
        }

        return alt;
      }

      return null;
    }

    private static string GetImageDescriptionFromThumbnail(HtmlNode node)
    {
      var parentNode = node.Ancestors().SingleOrDefault(x => x.GetAttributeValue("class", null) == "tsingle")
        ?? node.Ancestors().SingleOrDefault(x => x.GetAttributeValue("class", null) == "thumbinner");

      var captionNode = parentNode?.Descendants().FirstOrDefault(x => x.GetAttributeValue("class", null) == "thumbcaption");
      if (captionNode != null)
        return ExtractImageDescriptionFromNode(captionNode);

      return null;
    }

    private static string GetImageDescriptionFromGallery(HtmlNode node)
    {
      var parentNode = node.Ancestors().SingleOrDefault(x => x.GetAttributeValue("class", null) == "gallerybox");

      var captionNode = parentNode?.Descendants().SingleOrDefault(x => x.GetAttributeValue("class", null) == "gallerytext");
      if (captionNode != null)
        return ExtractImageDescriptionFromNode(captionNode);

      return null;
    }

    private static string ExtractImageDescriptionFromNode(HtmlNode node)
    {
      var removeNodes = node.ChildNodes.Where(x => (x.Name == "div" && x.HasClass("magnify")) || x.Name == "sup").ToArray();
      foreach (var removeNode in removeNodes)
      {
        node.RemoveChild(removeNode);
      }

      return RemoveHtml(node.InnerText).Trim(' ', ':');
    }

    private class SiteinfoRoot
    {
      public SiteinfoQuery query { get; set; }
    }

    private class SiteinfoQuery
    {
      public SiteinfoGeneral general { get; set; }
    }

    private class SiteinfoGeneral
    {
      public string mainpage { get; set; }
    }

    private class PagesRoot
    {
      public PagesQuery query { get; set; }
    }

    private class PagesQuery
    {
      public List<PagesPage> pages { get; set; }
    }

    private class PagesPage
    {
      public int pageid { get; set; }
      public string pagelanguage { get; set; }
      public string title { get; set; }
      public string fullurl { get; set; }
      public string description { get; set; }
      public Thumbnail thumbnail { get; set; }
    }

    private class Thumbnail
    {
      public string source { get; set; }
    }

    private class ImagesGeneratorRoot
    {
      public ImagesGeneratorContinue @continue { get; set; }
      public ImagesGeneratorQuery query { get; set; }
    }

    private class ImagesGeneratorContinue
    {
      public string gimcontinue { get; set; }
      public string @continue { get; set; }
    }

    private class ImagesGeneratorQuery
    {
      public List<ImagesGeneratorPage> pages { get; set; }
    }

    private class ImagesGeneratorPage
    {
      public List<ImagesGeneratorImageInfo> imageinfo { get; set; }
      public List<Revision> revisions { get; set; }
    }

    private class ImagesGeneratorImageInfo
    {
      public string thumburl { get; set; }
      public string url { get; set; }
      public string descriptionurl { get; set; }
    }

    private class Revision
    {
      public Dictionary<string, RevisionSlot> slots { get; set; }
    }

    private class RevisionSlot
    {
      public string content { get; set; }
    }
  }

  public class WikipediaParseApi : WikipediaApi
  {
    public async Task<Article> FetchArticle(string language, int? pageId, string title, bool disableImages, string anchor = null, ArticleHead head = null, Article article = null)
    {
      var query = "action=parse&prop=text|sections|langlinks|images|headhtml&disableeditsection=&disabletoc=&disablelimitreport=&useskin=minerva";
      if (pageId != null)
        query += "&pageid=" + pageId;
      else
        query += "&page=" + title + "&redirects=";

      if (disableImages)
        query += "&noimages=";

      var rootObject = await QueryAndParse<ParseRoot>(language, query);
      var parseResult = rootObject?.parse;
      if (parseResult == null)
        return null;

      var sections = new List<ArticleSection>();
      if (parseResult.sections != null && parseResult.sections.Count > 0)
      {
        foreach (var section in parseResult.sections)
        {
          sections.Add(new ArticleSection
          {
            Level = section.toclevel,
            Number = section.number,
            Headline = !string.IsNullOrEmpty(section.line) ? RemoveHtml(section.line) : null,
            Anchor = section.anchor
          });
        }
      }

      var languages = new List<ArticleLanguage>();
      if (parseResult.langlinks != null && parseResult.langlinks.Count > 0)
      {
        foreach (var langlink in parseResult.langlinks)
        {
          languages.Add(new ArticleLanguage
          {
            Code = langlink.lang,
            Name = string.IsNullOrEmpty(langlink.autonym) ? langlink.langname : langlink.autonym,
            LocalizedName = langlink.langname,
            Title = langlink.title,
            Uri = new Uri(langlink.url),
          });
        }
      }

      var images = new List<string>();
      if (parseResult.images?.Count > 0)
      {
        images.AddRange(parseResult.images);
      }

      if (article == null)
        article = new Article(head);

      article.Html = BuildHtml(language, parseResult.title, head != null ? head.Description : article.Description, parseResult.text, parseResult.headhtml);
      article.Title = parseResult.title;
      article.Sections = sections;
      article.Languages = languages;
      article.Images = images;
      article.Anchor = anchor;

      return article;
    }

    private static string BuildHtml(string language, string title, string description, string content, string headHtml)
    {
      var textDirection = ParseDirection(headHtml);

      var html = $@"
        {headHtml}
        <div id=""mw-mf-viewport"">
	        <div id=""mw-mf-page-center"">
		        <div id=""content"" class=""mw-body"">
              <div class=""pre-content heading-holder"">
                <h1 id=""section_0"">{title}</h1>
                <span style=""font-size:0.8125em;line-height:1.5;color:gray"">{description}</span>
                <hr style=""margin-bottom:20px;width:100px;height:1px;text-align:left;border:none;background:gray"">
              </div><div id=""bodyContent"" class=""content""><div id=""mw-content-text"" lang=""{language}"" dir=""{textDirection}"" class=""mw-content-{textDirection}""><script>function mfTempOpenSection(id){{var block=document.getElementById(""mf-section-""+id);block.className+="" open-block"";block.previousSibling.className+="" open-block"";}}</script>
        {content}
        </div></div></div></div></div></div>
        </body>
        </html>
      ";

      var document = new HtmlDocument();
      document.LoadHtml(html);

      var baseNode = document.CreateElement("base");
      baseNode.SetAttributeValue("href", $"https://{language}.m.wikipedia.org");
      document.DocumentNode.Descendants("head").Single().PrependChild(baseNode);

      var rootNode = document.DocumentNode.Descendants("div").Single(x => x.HasClass("mw-parser-output"));

      var nodes = rootNode.ChildNodes.Where(x => x.HasClass("authority-control") || x.HasClass("portal-bar") || x.HasClass("sister-bar")).ToList();
      nodes.ForEach(x => x.Remove());

      var rootChildren = rootNode.ChildNodes.ToArray();

      var sectionNode = document.CreateElement("section");
      sectionNode.AddClass("mf-section-0");
      var sectionCount = 0;

      foreach (var child in rootChildren)
      {
        if (child.OriginalName == "h2")
        {
          rootNode.InsertBefore(sectionNode, child);

          sectionNode = document.CreateElement("section");
          sectionNode.AddClass($"mf-section-{++sectionCount}");
        }
        else
        {
          sectionNode.MoveChild(child);
        }
      }

      rootNode.AppendChild(sectionNode);


      return document.DocumentNode.OuterHtml;
    }

    private static string ParseDirection(string html)
    {
      var offset = html.IndexOf(" dir=\"", StringComparison.OrdinalIgnoreCase);

      return html.Substring(offset + 6, 3);
    }

    private class ParseRoot
    {
      public ParseResult parse { get; set; }
    }

    private class ParseResult
    {
      public string title { get; set; }
      public int pageid { get; set; }
      public string headhtml { get; set; }
      public string text { get; set; }
      public List<ParseSection> sections { get; set; }
      public List<ParseLanglink> langlinks { get; set; }
      public List<string> images { get; set; }
    }

    private class ParseLanglink
    {
      public string lang { get; set; }
      public string url { get; set; }
      public string langname { get; set; }
      public string autonym { get; set; }
      public string title { get; set; }
    }

    private class ParseSection
    {
      public int toclevel { get; set; }
      public string line { get; set; }
      public string number { get; set; }
      public string anchor { get; set; }
    }
  }

  public class WikipediaSearchApi : WikipediaApi
  {
    private const int MaxResults = 10;

    public async Task<IList<FoundArticle>> PrefixSearch(string searchTerm, string language, CancellationToken? cancellationToken)
    {
      var query = $"action=query&redirects=&converttitles=&prop=description|pageimages|info&piprop=thumbnail&pilicense=any&generator=prefixsearch&gpsnamespace=0&list=search&srnamespace=0&inprop=url&srwhat=text&srinfo=suggestion&srprop=&sroffset=0&srlimit=1&pithumbsize={ThumbnailSize}&gpssearch={searchTerm}&gpslimit={MaxResults}&srsearch={searchTerm}";
      var response = await QueryAndParse<SearchResponse>(language, query, cancellationToken);

      var pages = response?.query?.pages;

      return MapPages(pages);
    }

    public async Task<IList<FoundArticle>> FullTextSearch(string searchTerm, string language, CancellationToken? cancellationToken)
    {
      var query = $"action=query&converttitles=&prop=description|pageimages|info&generator=search&gsrnamespace=0&gsrwhat=text&inprop=url&gsrinfo=&gsrprop=redirecttitle&piprop=thumbnail&pilicense=any&pithumbsize={ThumbnailSize}&gsrsearch={searchTerm}&gsrlimit={MaxResults}";
      var response = await QueryAndParse<SearchResponse>(language, query, cancellationToken);

      var pages = response?.query?.pages;

      return MapPages(pages);
    }

    private static IList<FoundArticle> MapPages(List<Page> pages)
    {
      var list = new List<FoundArticle>();

      if (pages != null && pages.Count > 0)
      {
        foreach (var page in pages.OrderBy(x => x.index))
        {
          var article = new FoundArticle
          {
            PageId = page.pageid,
            Title = page.title,
            Description = page.description,
            Language = page.pagelanguage,
            Uri = new Uri(page.fullurl),
            ThumbnailUri = page.thumbnail != null ? new Uri(page.thumbnail.source) : null
          };

          list.Add(article);
        }
      }

      return list;
    }

    private class SearchResponse
    {
      public SearchQuery query { get; set; }
    }

    private class SearchQuery
    {
      public List<Page> pages { get; set; }
    }

    private class Page
    {
      public int pageid { get; set; }
      public string title { get; set; }
      public int index { get; set; }
      public string description { get; set; }
      public string fullurl { get; set; }
      public string pagelanguage { get; set; }
      public Thumbnail thumbnail { get; set; }
    }

    private class Thumbnail
    {
      public string source { get; set; }
    }
  }

  public class WikipediaGeosearchApi : WikipediaApi
  {
    public async Task<IList<NearbyArticle>> Search(double latitude, double longitude, string language)
    {
      var pages = new List<Page>();

      await GeoSearch(language, latitude, longitude, null, pages);

      return MapPages(language, pages);
    }

    private async Task GeoSearch(string language, double latitude, double longitude, string cocontinue, List<Page> pages)
    {
      var query = $"action=query&generator=geosearch&prop=coordinates&ggscoord={latitude.ToString(CultureInfo.InvariantCulture)}|{longitude.ToString(CultureInfo.InvariantCulture)}&ggslimit=30&ggsradius=5000";

      if (!string.IsNullOrEmpty(cocontinue))
        query += $"&cocontinue={cocontinue}";

      var response = await QueryAndParse<GeosearchResponse>(language, query);

      if (response?.query?.pages != null)
      {
        foreach (var page in response.query.pages)
        {
          if (page.coordinates == null || page.coordinates.Count == 0)
            continue;

          pages.Add(page);
        }
      }

      if (!string.IsNullOrEmpty(response?.@continue?.cocontinue))
        await GeoSearch(language, latitude, longitude, response.@continue.cocontinue, pages);
    }

    public async Task<NearbyArticle> GetArticleLocation(string language, int? pageId, string title, int thumbnailSize)
    {
      var query = $"action=query&prop=info|description|pageimages|coordinates|extracts&inprop=url&exintro=&explaintext=&pilicense=any&pilimit=1&pithumbsize={thumbnailSize}";
      if (pageId != null)
        query += "&pageids=" + pageId;
      else
        query += "&titles=" + title + "&redirects=";

      var result = await QueryAndParse<GeosearchResponse>(language, query);

      var pages = result?.query?.pages;
      if (pages == null || pages.Count == 0)
        return null;

      var page = pages.First();
      var coordinates = page.coordinates.FirstOrDefault();

      return new NearbyArticle
      {
        Language = page.pagelanguage,
        PageId = page.pageid,
        Title = page.title,
        Description = page.description,
        Uri = new Uri(page.fullurl),
        ThumbnailUri = !string.IsNullOrEmpty(page.thumbnail?.source) ? new Uri(page.thumbnail.source) : null,
        Coordinates = coordinates != null ? new Coordinates(coordinates.lat, coordinates.lon) : null,
        Excerpt = page.extract
      };
    }

    private static IList<NearbyArticle> MapPages(string language, List<Page> pages)
    {
      var list = new List<NearbyArticle>();

      foreach (var page in pages)
      {
        var coordinates = page.coordinates.First();

        list.Add(new NearbyArticle
        {
          Language = language,
          PageId = page.pageid,
          Title = page.title,
          Coordinates = new Coordinates(coordinates.lat, coordinates.lon)
        });
      }

      return list;
    }

    private class GeosearchResponse
    {
      public Continue @continue { get; set; }
      public Query query { get; set; }
    }

    private class Continue
    {
      public string cocontinue { get; set; }
    }

    private class Query
    {
      public List<Page> pages { get; set; }
    }

    private class Page
    {
      public int pageid { get; set; }
      public string title { get; set; }
      public string description { get; set; }
      public string fullurl { get; set; }
      public string pagelanguage { get; set; }
      public Thumbnail thumbnail { get; set; }
      public List<LatituteLongitude> coordinates { get; set; }
      public string extract { get; set; }
    }

    private class Thumbnail
    {
      public string source { get; set; }
    }

    private class LatituteLongitude
    {
      public double lat { get; set; }
      public double lon { get; set; }
    }
  }

  public abstract class WikipediaApi
  {
    protected const int ThumbnailSize = 300;

    protected Task<T> QueryAndParse<T>(string language, string query, CancellationToken? cancellationToken = null)
    {
      var url = $"https://{language}.wikipedia.org/w/api.php?format=json&formatversion=2&{query}";

      return QueryAndParse<T>(url, cancellationToken);
    }

    protected async Task<T> QueryAndParse<T>(string url, CancellationToken? cancellationToken = null)
    {
      var requestUri = new Uri(url);

      var response = await SendRequest(requestUri, cancellationToken);
      var result = JsonConvert.DeserializeObject<T>(response);

      return result;
    }

    private static async Task<string> SendRequest(Uri requestUri, CancellationToken? cancellationToken)
    {
      using (var client = new HttpClient())
      {
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
        {
          var operation = client.TryGetStringAsync(requestUri);

          var response = cancellationToken != null ? await operation.AsTask(cancellationToken.Value) : await operation;

          return response.Value;
        }
        else
        {
          var operation = client.GetStringAsync(requestUri);

          try
          {
            var response = cancellationToken != null ? await operation.AsTask(cancellationToken.Value) : await operation;

            return response;
          }
          catch (Exception)
          {
            return string.Empty;
          }
        }
      }
    }

    public static string RemoveHtml(string text)
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