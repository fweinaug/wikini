using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WikipediaApp
{
  public interface IWikipediaService
  {
    Task<Article> GetArticle(ArticleHead data, bool disableImages);
    Task<Article> GetArticle(Uri uri, bool disableImages);

    Task<ArticleHead> GetArticleInfo(string language, int? pageId, string title);
    Task<ArticleHead> GetArticleInfo(Uri uri);
    Task<Uri> GetArticleUri(string language, int? pageId, string title);
    Task<IList<ArticleImage>> GetArticleImages(Article article);
    Task<NearbyArticle> GetArticleLocation(string language, int? pageId, string title, int thumbnailSize = 350);

    Task<ArticleHead> GetMainPage(string language);
    Task<ArticleHead> GetRandomArticle(string language);
    Task<ArticleImage> GetPictureOfTheDay(DateTime date);

    Task<bool> PinArticle(ArticleHead article);
    Task<bool> PinArticle(string language, int? pageId, string title);

    Task<bool> AddArticleToTimeline(ArticleHead article);

    Task<Article> RefreshArticle(Article article, bool disableImages);

    Task<IList<FoundArticle>> Search(string searchTerm, string language, CancellationToken? cancellationToken);
    Task<IList<NearbyArticle>> SearchNearby(string language, double latitude, double longitude);
  }
}