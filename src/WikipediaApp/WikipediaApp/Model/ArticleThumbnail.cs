using System;

namespace WikipediaApp
{
  public class ArticleThumbnail
  {
    public int PageId { get; set; }
    public string Title { get; set; }
    public Uri ImageUri { get; set; }
  }
}