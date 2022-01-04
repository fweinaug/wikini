using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace WikipediaApp
{
  public class LanguageViewModel : ObservableObject
  {
    private bool isFavorite = false;
    private bool isActive = false;

    private RelayCommand addToFavoritesCommand;
    private RelayCommand removeFromFavoritesCommand;

    public string Name { get; }
    public string Code { get; }
    public Uri Uri { get; }
    public int Index { get; }

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

    public ICommand AddToFavoritesCommand
    {
      get { return addToFavoritesCommand ?? (addToFavoritesCommand = new RelayCommand(AddToFavorites)); }
    }

    public ICommand RemoveFromFavoritesCommand
    {
      get { return removeFromFavoritesCommand ?? (removeFromFavoritesCommand = new RelayCommand(RemoveFromFavorites)); }
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

      WeakReferenceMessenger.Default.Register<LanguageViewModel, LanguageIsFavoriteChanged>(this, (_, message) =>
      {
        if (message.Code == Code)
        {
          IsFavorite = message.IsFavorite;
        }
      });
    }

    private void AddToFavorites()
    {
      WeakReferenceMessenger.Default.Send(new AddLanguageToFavorites(Code));
    }

    private void RemoveFromFavorites()
    {
      WeakReferenceMessenger.Default.Send(new RemoveLanguageFromFavorites(Code));
    }
  }
}