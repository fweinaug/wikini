using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using HtmlAgilityPack;
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

    public async Task<ArticleThumbnail> GetArticleThumbnail(string language, int? pageId, string title)
    {
      try
      {
        var query = "action=query&prop=pageimages&pilicense=any&pilimit=1&pithumbsize=300";
        if (pageId != null)
          query += "&pageids=" + pageId;
        else
          query += "&titles=" + title + "&redirects=";

        var result = await QueryAndParse<PageimagesRoot>(language, query);

        var pages = result?.query?.pages;
        if (pages == null || pages.Count == 0)
          return null;

        var page = pages.First();

        return new ArticleThumbnail
        {
          PageId = page.pageid,
          Title = page.title,
          ImageUri = page.thumbnail?.source
        };
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    public async Task<List<ArticleImage>> GetArticleImages(Article article)
    {
      var imagesByFilenameDictionary = await FetchArticleImagesGroupedByFilename(article.PageId, article.Language);

      var images = ParseArticleForImages(article, imagesByFilenameDictionary);
      return images;
    }

    private async Task<Dictionary<string, ArticleImage>> FetchArticleImagesGroupedByFilename(int pageId, string language)
    {
      var dictionary = new Dictionary<string, ArticleImage>();

      var query = "action=query&generator=images&gimlimit=50&prop=imageinfo&iiprop=url&iiurlwidth=800&iiurlheight=800";
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

          var imageUri = new Uri(imageinfo.url);
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
      document.LoadHtml(article.Content);

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

      var captionNode = parentNode?.Descendants().SingleOrDefault(x => x.GetAttributeValue("class", null) == "thumbcaption");
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
      var removeNodes = node.ChildNodes.Where(x => x.Name == "div" || x.Name == "sup").ToArray();
      foreach (var removeNode in removeNodes)
      {
        node.RemoveChild(removeNode);
      }

      return RemoveHtml(node.InnerText).Trim(' ', ':');
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
      public List<RandomPage> pages { get; set; }
    }

    private class RandomPage
    {
      public int pageid { get; set; }
      public int ns { get; set; }
      public string title { get; set; }
      public string fullurl { get; set; }
    }

    private class PageimagesRoot
    {
      public bool batchcomplete { get; set; }
      public PageimagesQuery query { get; set; }
    }

    private class PageimagesQuery
    {
      public List<PageimagesPage> pages { get; set; }
    }

    private class PageimagesPage
    {
      public int pageid { get; set; }
      public int ns { get; set; }
      public string title { get; set; }
      public Thumbnail thumbnail { get; set; }
      public string pageimage { get; set; }
    }

    private class Thumbnail
    {
      public Uri source { get; set; }
      public int width { get; set; }
      public int height { get; set; }
    }

    private class ImagesGeneratorRoot
    {
      public bool batchcomplete { get; set; }
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
      public int ns { get; set; }
      public string title { get; set; }
      public bool missing { get; set; }
      public bool known { get; set; }
      public string imagerepository { get; set; }
      public List<ImagesGeneratorImageInfo> imageinfo { get; set; }
    }

    private class ImagesGeneratorImageInfo
    {
      public string thumburl { get; set; }
      public string url { get; set; }
      public string descriptionurl { get; set; }
      public string descriptionshorturl { get; set; }
    }
  }

  public class WikipediaParseApi : WikipediaApi
  {
    public async Task<Article> FetchArticle(string language, Uri uri, bool disableImages, int? pageId = null, string title = null, string anchor = null, Article article = null)
    {
      var query = "action=parse&prop=text|sections|langlinks|images&disableeditsection=&disabletoc=&mobileformat=";
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
        foreach (var langling in parseResult.langlinks)
        {
          languages.Add(new ArticleLanguage
          {
            Name = string.IsNullOrEmpty(langling.autonym) ? langling.langname : langling.autonym,
            Uri = new Uri(langling.url)
          });
        }
      }

      var images = new List<string>();
      if (parseResult.images?.Count > 0)
      {
        images.AddRange(parseResult.images);
      }

      if (article == null)
        article = new Article();

      article.Language = language;
      article.PageId = parseResult.pageid;
      article.Title = parseResult.title;
      article.Content = parseResult.text;
      article.Uri = uri;
      article.Sections = sections;
      article.Languages = languages;
      article.Images = images;
      article.Anchor = anchor;

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
    public async Task<IList<ArticleHead>> Search(string searchTerm, string language, CancellationToken? cancellationToken)
    {
      var list = new List<ArticleHead>();

      var query = "action=opensearch&search=" + searchTerm + "&namespace=0&redirects=resolve&limit=10";
      var response = await SendRequest(language, query, cancellationToken);
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

    protected async Task<string> SendRequest(string language, string query, CancellationToken? cancellationToken = null)
    {
      var requestUri = new Uri("https://" + language + ".wikipedia.org/w/api.php?format=json&formatversion=2&" + query);

      var client = new HttpClient();
      var operation = client.GetStringAsync(requestUri);

      var response = cancellationToken != null ? await operation.AsTask(cancellationToken.Value) : await operation;

      return response;
    }

    protected static string RemoveHtml(string text)
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