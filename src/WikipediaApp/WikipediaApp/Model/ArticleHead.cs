using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WikipediaApp
{
  public class ArticleHead
  {
    private string url = null;
    private Uri uri = null;

    private string thumbnailUrl = null;
    private Uri thumbnailUri = null;

    public int? PageId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }

    [NotMapped]
    public bool HasDescription
    {
      get { return !string.IsNullOrEmpty(Description); }
    }

    [NotMapped]
    public bool HasThumbnail
    {
      get { return ThumbnailUri != null; }
    }

    [NotMapped]
    public Uri Uri
    {
      get
      {
        if (uri == null && !string.IsNullOrEmpty(url))
        {
          uri = new Uri(url);
        }

        return uri;
      }
      set
      {
        uri = value;
        url = value?.ToString();
      }
    }

    [NotMapped]
    public Uri ThumbnailUri
    {
      get
      {
        if (thumbnailUri == null && !string.IsNullOrEmpty(thumbnailUrl))
        {
          thumbnailUri = new Uri(thumbnailUrl);
        }

        return thumbnailUri;
      }
      set
      {
        thumbnailUri = value;
        thumbnailUrl = value?.ToString();
      }
    }
  }
}