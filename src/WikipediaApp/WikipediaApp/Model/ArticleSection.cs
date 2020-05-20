using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WikipediaApp
{
  public class ArticleSection : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private bool isActive = false;

    public int Level { get; set; }
    public string Number { get; set; }
    public string Headline { get; set; }
    public string Anchor { get; set; }

    public bool IsRoot => Level == 1;

    public bool IsNext => IsRoot && int.Parse(Number) > 1;

    public bool IsActive
    {
      get => isActive;
      set
      {
        if (isActive != value)
        {
          isActive = value;
          RaisePropertyChanged();
        }
      }
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}