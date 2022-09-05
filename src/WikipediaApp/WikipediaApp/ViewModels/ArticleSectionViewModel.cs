using CommunityToolkit.Mvvm.ComponentModel;

namespace WikipediaApp
{
  public class ArticleSectionViewModel : ObservableObject
  {
    private bool isActive = false;

    public int Level { get; private set; }
    public string Number { get; private set; }
    public string Headline { get; private set; }
    public string Anchor { get; private set; }

    public bool IsRoot => Level == 1;
    public bool IsNext => IsRoot && int.Parse(Number) > 1;

    public bool IsActive
    {
      get => isActive;
      set { SetProperty(ref isActive, value); }
    }

    public ArticleSectionViewModel(ArticleSection section)
    {
      Level = section.Level;
      Headline = section.Headline;
      Number = section.Number;
      Anchor = section.Anchor;
    }
  }
}