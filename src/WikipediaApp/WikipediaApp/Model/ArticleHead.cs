using System;

namespace WikipediaApp
{
  public class ArticleHead
  {
    public int? Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public Uri Uri { get; set; }
  }
}