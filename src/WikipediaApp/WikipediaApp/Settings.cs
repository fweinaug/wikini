using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace WikipediaApp
{
  public class Settings : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public const int DefaultAppTheme = 0;
    public const string DefaultSearchLanguage = "en";
    public const bool DefaultSectionsCollapsed = false;
    public const bool DefaultSearchRestricted = false;

    public static Settings Current { get; private set; }

    private readonly ApplicationDataContainer container;

    public int AppTheme
    {
      get { return GetValue(DefaultAppTheme); }
      set { SetValue(value); }
    }

    public string SearchLanguage
    {
      get { return GetValue(DefaultSearchLanguage); }
      set { SetValue(value); }
    }

    public bool SectionsCollapsed
    {
      get { return GetValue(DefaultSectionsCollapsed); }
      set { SetValue(value); }
    }

    public bool SearchRestricted
    {
      get { return GetValue(DefaultSearchRestricted); }
      set { SetValue(value); }
    }

    public int FontSize
    {
      get { return GetValue(15); }
      set { SetValue(value); }
    }

    public bool ShowThanks
    {
      get { return GetValue(false); }
      set { SetValue(value); }
    }

    public Settings()
    {
      Current = this;

      container = ApplicationData.Current.RoamingSettings;
    }

    private T GetValue<T>(T defaultValue, [CallerMemberName]string name = null)
    {
      return (T)(container.Values[name] ?? defaultValue);
    }

    private void SetValue(object value, [CallerMemberName]string name = null)
    {
      if (container.Values[name] == value)
        return;

      container.Values[name] = value;

      RaisePropertyChanged(name);
    }

    private void RaisePropertyChanged(string name)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        var args = new PropertyChangedEventArgs(name);
        handler(this, args);
      }
    }
  }
}