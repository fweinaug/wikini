using System;

namespace WikipediaApp
{
  public class ArticleImage
  {
    public string Name { get; set; }
    public Uri ImageUri { get; set; }
    public Uri ThumbnailUri { get; set; }
    public string Description { get; set; }

    public bool HasDescription
    {
      get { return !string.IsNullOrEmpty(Description); }
    }
  }
}