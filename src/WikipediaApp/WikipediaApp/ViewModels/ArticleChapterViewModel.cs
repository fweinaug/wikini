using CommunityToolkit.Mvvm.ComponentModel;

namespace WikipediaApp
{
  public class ArticleChapterViewModel : ObservableObject
  {
    private bool isActive = false;
    private bool isLoading = false;
    private bool isPlaying = false;

    public int Level { get; set; }
    public string Number { get; set; }
    public string Headline { get; set; }
    public string Content { get; set; }
    public string Anchor { get; set; }

    public bool IsActive
    {
      get { return isActive; }
      set { SetProperty(ref isActive, value); }
    }

    public bool IsLoading
    {
      get { return isLoading; }
      set { SetProperty(ref isLoading, value); }
    }

    public bool IsPlaying
    {
      get { return isPlaying; }
      set { SetProperty(ref isPlaying, value); }
    }

    public bool IsRoot
    {
      get { return Level == 1; }
    }

    public bool NewSection => IsRoot && HasNumber && int.Parse(Number) > 1;

    public bool HasNumber
    {
      get { return !string.IsNullOrEmpty(Number); }
    }
  }
}