using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;

namespace WikipediaApp
{
  public class WikipediaService
  {
    private static readonly ArticleCache<ArticleHead> articleHeadCache = new ArticleCache<ArticleHead>();
    private static readonly ArticleCache<Article> articleCache = new ArticleCache<Article>();
    private static readonly ArticleCache<NearbyArticle> articleLocationCache = new ArticleCache<NearbyArticle>();
    private static readonly Dictionary<DateTime, ArticleImage> pictureOfTheDayCache = new Dictionary<DateTime, ArticleImage>();

    private readonly WikipediaSearchApi searchApi = new WikipediaSearchApi();
    private readonly WikipediaGeosearchApi geosearchApi = new WikipediaGeosearchApi();
    private readonly WikipediaQueryApi queryApi = new WikipediaQueryApi();
    private readonly WikipediaParseApi parseApi = new WikipediaParseApi();

    public async Task<Article> GetArticle(Uri uri, bool disableImages)
    {
      string title, language, anchor;
      if (!WikipediaUriParser.Parse(uri, out title, out language, out anchor))
        return null;

      if (string.IsNullOrEmpty(title))
      {
        var header = await GetMainPage(language);
        if (header == null)
          return null;

        return await GetArticle(header, disableImages);
      }

      try
      {
        var article = articleCache.FindArticle(uri, language);
        if (article != null)
          return article;

        var head = await GetArticleInfo(language, null, title);
        if (head == null)
          return null;

        article = await parseApi.FetchArticle(head.Language, head.PageId, head.Title, disableImages, anchor, head);

        if (article != null)
          articleCache.AddArticle(article);

        return article;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<Uri> GetArticleUri(string language, int? pageId, string title)
    {
      try
      {
        var article = await GetArticleInfo(language, pageId, title);

        return article?.Uri;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<ArticleHead> GetArticleInfo(string language, int? pageId, string title)
    {
      try
      {
        var article = articleHeadCache.FindArticle(language, pageId, title);
        if (article != null)
          return article;

        article = await queryApi.GetArticleInfo(language, pageId, title);

        if (article != null)
          articleHeadCache.AddArticle(article);

        return article;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<ArticleHead> GetArticleInfo(Uri uri)
    {
      try
      {
        if (!WikipediaUriParser.Parse(uri, out var title, out var language, out _))
          return null;

        var article = await GetArticleInfo(language, null, title);

        return article;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<IList<ArticleImage>> GetArticleImages(Article article)
    {
      try
      {
        return await queryApi.GetArticleImages(article);
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public bool IsWikipediaUri(Uri uri)
    {
      return WikipediaUriParser.IsWikipediaUri(uri);
    }

    public bool IsLinkToWikipediaImage(Uri uri, out string filename)
    {
      if (uri.Scheme == "about" && uri.AbsolutePath == "blank")
      {
        var fragment = Uri.UnescapeDataString(uri.Fragment).Trim('#');
        if (fragment.StartsWith("/media/"))
        {
          var index = fragment.IndexOf(':');
          if (index > 0)
          {
            filename = fragment.Substring(index + 1);
            return true;
          }
        }
      }

      if (uri.Host.EndsWith(".wikipedia.org") && uri.AbsolutePath.StartsWith("/wiki/"))
      {
        var index = uri.AbsolutePath.IndexOf(':', 6);
        if (index > 0)
        {
          filename = WebUtility.UrlDecode(uri.AbsolutePath.Substring(index + 1));
          return true;
        }
      }

      filename = null;
      return false;
    }

    public async Task<ArticleHead> GetMainPage(string language)
    {
      try
      {
        return await queryApi.GetMainPage(language);
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<ArticleHead> GetRandomArticle(string language)
    {
      try
      {
        return await queryApi.GetRandomArticle(language);
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<Article> GetArticle(ArticleHead data, bool disableImages)
    {
      try
      {
        var article = articleCache.FindArticle(data.Uri, data.Language, data.PageId);
        if (article != null)
          return article;

        var head = await GetArticleInfo(data.Language, data.PageId, data.Title);
        article = await parseApi.FetchArticle(data.Language, data.PageId, data.Title, disableImages, head: head);

        if (article != null)
          articleCache.AddArticle(article);

        return article;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<Article> RefreshArticle(Article article, bool disableImages)
    {
      try
      {
        return await parseApi.FetchArticle(article.Language, article.PageId, article.Title, disableImages, article.Anchor, article: article);
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<IList<FoundArticle>> Search(string searchTerm, string language, CancellationToken? cancellationToken)
    {
      try
      {
        var pages = await searchApi.PrefixSearch(searchTerm, language, cancellationToken);

        if (pages.Count == 0)
          pages = await searchApi.FullTextSearch(searchTerm, language, cancellationToken);

        return pages;
      }
      catch (TaskCanceledException)
      {
        return null;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex, new Dictionary<string, string>
        {
          { "searchTerm", searchTerm },
          { "language", language }
        });

        return null;
      }
    }

    public async Task<bool> PinArticle(string language, int? pageId, string title)
    {
      try
      {
        var article = await GetArticleInfo(language, pageId, title);

        if (article != null)
          return await TileManager.PinArticle(language, article.PageId.GetValueOrDefault(), article.Title, article.Uri, article.ThumbnailUri);

        return false;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return false;
      }
    }

    public async Task<bool> PinArticle(ArticleHead article)
    {
      try
      {
        return await TileManager.PinArticle(article.Language, article.PageId.GetValueOrDefault(), article.Title, article.Uri, article.ThumbnailUri);
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return false;
      }
    }

    public async Task<bool> AddArticleToTimeline(ArticleHead article)
    {
      try
      {
        return await TimelineManager.AddArticle(article.Language, article.PageId.GetValueOrDefault(), article.Title, article.Uri, article.ThumbnailUri);
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return false;
      }
    }

    public async Task<ArticleImage> GetPictureOfTheDay(DateTime date)
    {
      try
      {
        if (pictureOfTheDayCache.TryGetValue(date, out var cachedImage))
        {
          return cachedImage;
        }

        var image = await queryApi.GetPictureOfTheDay(date);

        pictureOfTheDayCache.Add(date, image);

        return image;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<IList<NearbyArticle>> SearchNearby(string language, double latitude, double longitude)
    {
      try
      {
        return await geosearchApi.Search(latitude, longitude, language);
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }

    public async Task<NearbyArticle> GetArticleLocation(string language, int? pageId, string title, int thumbnailSize = 350)
    {
      try
      {
        var article = articleLocationCache.FindArticle(language, pageId, title);
        if (article != null)
          return article;

        article = await geosearchApi.GetArticleLocation(language, pageId, title, thumbnailSize);

        if (article != null)
          articleLocationCache.AddArticle(article);

        return article;
      }
      catch (Exception ex)
      {
        Crashes.TrackError(ex);

        return null;
      }
    }
  }
}