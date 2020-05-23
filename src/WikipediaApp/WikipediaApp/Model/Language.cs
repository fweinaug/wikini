using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WikipediaApp
{
  public class Language : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private bool isFavorite = false;
    private bool isActive = false;

    public string Name { get; set; }
    public string Code { get; set; }
    public Uri Uri { get; set; }

    public bool IsFavorite
    {
      get { return isFavorite; }
      set
      {
        if (isFavorite != value)
        {
          isFavorite = value;
          OnPropertyChanged();
        }
      }
    }

    public bool IsActive
    {
      get => isActive;
      set
      {
        if (isActive != value)
        {
          isActive = value;
          OnPropertyChanged();
        }
      }
    }

    public int Index { get; set; }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}