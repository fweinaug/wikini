using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Microsoft.HockeyApp;

namespace WikipediaApp
{
  public partial class WikipediaService
  {
    private readonly WikipediaSearchApi searchApi = new WikipediaSearchApi();
    private readonly WikipediaQueryApi queryApi = new WikipediaQueryApi();
    private readonly WikipediaParseApi parseApi = new WikipediaParseApi();

    public async Task<IList<Language>> GetLanguages()
    {
      var uri = new Uri("ms-appx:///Data/languages.json");
      var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

      var content = await FileIO.ReadTextAsync(file);
      var array = JsonArray.Parse(content);

      var list = new List<Language>();

      foreach (var value in array)
      {
        var obj = value.GetObject();

        var visible = obj.GetNamedBoolean("Visible");
        if (!visible)
          continue;

        var name = obj.GetNamedString("Name");
        var code = obj.GetNamedString("Code");
        var url = obj.GetNamedString("Url");

        var language = new Language
        {
          Name = name,
          Code = code,
          Uri = new Uri(url)
        };

        list.Add(language);
      }

      return list;
    }

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
        return await parseApi.FetchArticle(language, uri, disableImages, title: title, anchor: anchor);
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    public async Task<Uri> GetArticleUri(string language, int? pageId, string title)
    {
      try
      {
        var article = await queryApi.GetArticleInfo(language, pageId, title);

        return article?.Uri;
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    public async Task<ArticleHead> GetArticleInfo(Uri uri)
    {
      if (!WikipediaUriParser.Parse(uri, out var title, out var language, out _))
        return null;

      var article = await queryApi.GetArticleInfo(language, null, title);

      return article;
    }

    public async Task<IList<ArticleImage>> GetArticleImages(Article article)
    {
      try
      {
        return await queryApi.GetArticleImages(article);
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

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
          filename = uri.AbsolutePath.Substring(index + 1);
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
        HockeyClient.Current.TrackException(ex);

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
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    public async Task<Article> GetArticle(ArticleHead article, bool disableImages)
    {
      try
      {
        return await parseApi.FetchArticle(article.Language, article.Uri, disableImages, title: article.Title, pageId: article.PageId);
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    public async Task<Article> RefreshArticle(Article article, bool disableImages)
    {
      try
      {
        return await parseApi.FetchArticle(article.Language, article.Uri, disableImages, article.PageId, anchor: article.Anchor, article: article);
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

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
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    public async Task<bool> PinArticle(string language, int? pageId, string title, Uri uri)
    {
      try
      {
        var thumbnail = await queryApi.GetArticleThumbnail(language, pageId, title);

        if (thumbnail != null)
          return await TileManager.PinArticle(language, thumbnail.PageId, thumbnail.Title, uri, thumbnail.ImageUri);

        return false;
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return false;
      }
    }

    public async Task<bool> AddArticleToTimeline(string language, int? pageId, string title, Uri uri)
    {
      try
      {
        var thumbnail = await queryApi.GetArticleThumbnail(language, pageId, title);

        if (thumbnail != null)
          return await TimelineManager.AddArticle(language, thumbnail.PageId, thumbnail.Title, uri, thumbnail.ImageUri);

        return false;
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return false;
      }
    }
  }
}