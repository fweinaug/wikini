using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation.Metadata;
using Windows.Storage;

namespace WikipediaApp
{
  public class Settings : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public const int DefaultAppTheme = 0;
    public const string DefaultSearchLanguage = "en";
    public const bool DefaultSearchRestricted = false;
    public const bool DefaultSectionsCollapsed = false;
    public const bool DefaultImagesDisabled = false;
    public const bool DefaultHistorySession = false;
    public const int DefaultFontSize = 15;

    public static Settings Current { get; private set; }

    private static readonly bool IsTimelineAvailable =
      ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5);

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

    public bool SearchRestricted
    {
      get { return GetValue(DefaultSearchRestricted); }
      set { SetValue(value); }
    }

    public bool SectionsCollapsed
    {
      get { return GetValue(DefaultSectionsCollapsed); }
      set { SetValue(value); }
    }

    public bool ImagesDisabled
    {
      get { return GetValue(DefaultImagesDisabled); }
      set { SetValue(value); }
    }

    public bool SplitViewInline
    {
      get { return GetValue(false); }
      set { SetValue(value); }
    }

    public bool HistorySession
    {
      get { return GetValue(DefaultHistorySession); }
      set { SetValue(value); }
    }

    public bool HistoryTimeline
    {
      get { return GetValue(IsTimelineAvailable); }
      set { SetValue(value); }
    }

    public int FontSize
    {
      get { return GetValue(DefaultFontSize); }
      set { SetValue(value); }
    }

    public bool StartHome
    {
      get { return GetValue(false); }
      set { SetValue(value); }
    }

    public bool StartPictureOfTheDay
    {
      get { return GetValue(true); }
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

    public static void WriteLastArticle(ArticleHead value)
    {
      var data = value != null ? value.PageId + "|" + value.Language + "|" + value.Title : null;

      ApplicationData.Current.LocalSettings.Values["LastArticle"] = data;
    }

    public static ArticleHead ReadLastArticle()
    {
      var data = ApplicationData.Current.LocalSettings.Values["LastArticle"] as string;
      if (string.IsNullOrEmpty(data))
        return null;

      var parts = data.Split('|');
      if (parts.Length < 3)
        return null;

      return new ArticleHead
      {
        PageId = parts[0].Length > 0 ? Convert.ToInt32(parts[0]) : (int?)null,
        Language = parts[1],
        Title = parts[2]
      };
    }
  }
}