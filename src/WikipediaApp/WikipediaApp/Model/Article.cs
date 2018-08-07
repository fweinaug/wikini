using System.Collections.Generic;
using System.Linq;

namespace WikipediaApp
{
  public class Article : ArticleHead
  {
    public new int PageId { get; set; }
    public string Content { get; set; }

    public List<ArticleSection> Sections { get; set; }
    public List<ArticleLanguage> Languages { get; set; }
    public List<string> Images { get; set; }

    public string Anchor { get; set; }

    public List<ArticleSection> GetRootSections()
    {
      if (Sections == null || Sections.Count == 0)
        return Sections;

      return Sections.Where(x => x.IsRoot).ToList();
    }
  }
}