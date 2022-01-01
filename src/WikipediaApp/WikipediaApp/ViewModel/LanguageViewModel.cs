using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace WikipediaApp
{
  public class LanguageViewModel : ObservableObject
  {
    private bool isFavorite = false;
    private bool isActive = false;

    public string Name { get; private set; }
    public string Code { get; private set; }
    public Uri Uri { get; private set; }
    public int Index { get; private set; }

    public bool IsFavorite
    {
      get { return isFavorite; }
      set { SetProperty(ref isFavorite, value); }
    }

    public bool IsActive
    {
      get => isActive;
      set { SetProperty(ref isActive, value); }
    }

    public LanguageViewModel()
    {
    }

    public LanguageViewModel(Language language, int index)
    {
      Name = language.Name;
      Code = language.Code;
      Uri = language.Uri;
      Index = index;
    }
  }
}