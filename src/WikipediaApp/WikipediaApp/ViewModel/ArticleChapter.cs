namespace WikipediaApp
{
  public class ArticleChapter : ViewModelBase
  {
    private bool isActive = false;
    private bool isLoading = false;
    private bool isPlaying = false;

    public int Level { get; set; }
    public string Number { get; set; }
    public string Headline { get; set; }
    public string Content { get; set; }

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

    public bool HasNumber
    {
      get { return !string.IsNullOrEmpty(Number); }
    }
  }
}