namespace WikipediaApp
{
  public interface IWikipediaContentBuilder
  {
    string GetContent(Article article, int header);
  }
}