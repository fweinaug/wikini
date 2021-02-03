namespace WikipediaApp
{
  public class NearbyArticle : ArticleHead
  {
    public Coordinates Coordinates { get; set; }

    public string Excerpt { get; set; }

    public bool HasExcerpt
    {
      get { return !string.IsNullOrEmpty(Excerpt); }
    }
  }
}