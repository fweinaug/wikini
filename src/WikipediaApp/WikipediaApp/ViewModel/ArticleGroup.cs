using System.Collections.ObjectModel;

namespace WikipediaApp
{
  public class ArticleGroup : ObservableCollection<ArticleHead>
  {
    public object Key { get; set; }
    public string Title { get; set; }
  }
}