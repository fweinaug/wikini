using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WikipediaApp
{
  public class ArticleHead : IEquatable<ArticleHead>
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

    public override bool Equals(object obj)
    {
      return Equals(obj as ArticleHead);
    }

    public bool Equals(ArticleHead other)
    {
      if (other == null)
        return false;

      if (PageId.HasValue && other.PageId.HasValue)
        return PageId == other.PageId && Language == other.Language;

      return Uri == other.Uri;
    }

    public static bool operator ==(ArticleHead left, ArticleHead right)
    {
      return EqualityComparer<ArticleHead>.Default.Equals(left, right);
    }

    public static bool operator !=(ArticleHead left, ArticleHead right)
    {
      return !(left == right);
    }
  }
}