using System.Collections.Generic;
using System.Threading.Tasks;

namespace WikipediaApp
{
  public interface IArticleHistoryRepository
  {
    Task<List<ReadArticle>> GetHistory();
    Task Clear();

    ReadArticle AddArticle(ArticleHead article);
    void RemoveArticle(ReadArticle article);
  }
}