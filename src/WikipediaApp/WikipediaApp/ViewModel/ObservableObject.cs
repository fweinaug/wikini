using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WikipediaApp
{
  public class ObservableObject : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(field, newValue))
        return false;

      field = newValue;
      RaisePropertyChanged(propertyName);

      return true;
    }

    protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}