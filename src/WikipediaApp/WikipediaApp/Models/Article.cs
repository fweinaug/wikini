using System.Collections.Generic;
using System.Linq;

namespace WikipediaApp
{
  public class Article : ArticleHead
  {
    public string Html { get; set; }

    public List<ArticleSection> Sections { get; set; }
    public List<ArticleLanguage> Languages { get; set; }
    public List<string> Images { get; set; }

    public string Anchor { get; set; }

    protected Article(Article article) : this((ArticleHead)article)
    {
      Html = article.Html;
      Sections = article.Sections;
      Languages = article.Languages;
      Images = article.Images;
      Anchor = article.Anchor;
    }

    public Article(ArticleHead article)
    {
      PageId = article.PageId;
      Title = article.Title;
      Description = article.Description;
      Language = article.Language;
      Uri = article.Uri;
      ThumbnailUri = article.ThumbnailUri;
    }

    public List<ArticleSection> GetRootSections()
    {
      if (Sections == null || Sections.Count == 0)
        return Sections;

      return Sections.Where(x => x.IsRoot).ToList();
    }
  }
}