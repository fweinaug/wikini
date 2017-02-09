using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Microsoft.HockeyApp;

namespace WikipediaApp
{
  public class WikipediaService
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

    public async Task<Article> GetArticle(Uri uri, bool darkMode, bool sectionsCollapsed)
    {
      string title, language;
      if (!ParseUri(uri, out title, out language))
        return null;

      if (string.IsNullOrEmpty(title))
      {
        var header = await GetMainPage(language);
        if (header == null)
          return null;

        return await GetArticle(header, darkMode, sectionsCollapsed);
      }

      try
      {
        return await parseApi.FetchArticle(language, darkMode, sectionsCollapsed, uri, title: title);
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    private bool ParseUri(Uri uri, out string title, out string language)
    {
      title = null;
      language = null;

      if (!IsWikipediaUri(uri))
        return false;

      if (!uri.AbsolutePath.StartsWith("/wiki/"))
        return false;

      title = uri.AbsolutePath.Substring(6);
      language = uri.Host.Substring(0, uri.Host.IndexOf('.'));

      return true;
    }

    public bool IsWikipediaUri(Uri uri)
    {
      return uri != null && uri.Host.EndsWith(".wikipedia.org");
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

    public async Task<Article> GetArticle(ArticleHead article, bool darkMode, bool sectionsCollapsed)
    {
      try
      {
        return await parseApi.FetchArticle(article.Language, darkMode, sectionsCollapsed, article.Uri, title: article.Title, pageId: article.Id);
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    public async Task<Article> RefreshArticle(Article article, bool darkMode, bool sectionsCollapsed)
    {
      try
      {
        return await parseApi.FetchArticle(article.Language, darkMode, sectionsCollapsed, article.Uri, article.PageId, article: article);
      }
      catch (Exception ex)
      {
        HockeyClient.Current.TrackException(ex);

        return null;
      }
    }

    public async Task<IList<ArticleHead>> Search(string searchTerm, string language, CancellationToken? cancellationToken)
    {
      try
      {
        return await searchApi.Search(searchTerm, language, cancellationToken);
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
  }
}