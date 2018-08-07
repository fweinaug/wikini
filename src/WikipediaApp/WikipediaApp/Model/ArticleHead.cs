using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WikipediaApp
{
  public class ArticleHead
  {
    private string url = null;
    private Uri uri = null;

    public int? PageId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }

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
  }
}