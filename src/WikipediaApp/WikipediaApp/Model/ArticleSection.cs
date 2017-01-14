namespace WikipediaApp
{
  public class ArticleSection
  {
    public int Level { get; set; }
    public string Number { get; set; }
    public string Headline { get; set; }
    public string Anchor { get; set; }

    public bool IsRoot
    {
      get { return Level == 1; }
    }
  }
}