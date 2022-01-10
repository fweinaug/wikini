using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace WikipediaApp
{
  public enum Typeface
  {
    SansSerif,
    Serif
  }

  public class Settings : ObservableObject
  {
    private readonly IUserSettings userSettings;

    public static Settings Current { get; private set; }

    public int AppTheme
    {
      get => userSettings.Get<int>(UserSettingsKey.AppTheme);
      set => userSettings.Set(UserSettingsKey.AppTheme, value);
    }

    public bool SectionsCollapsed
    {
      get => userSettings.Get<bool>(UserSettingsKey.SectionsCollapsed);
    }

    public Typeface Typeface
    {
      get => userSettings.Get<Typeface>(UserSettingsKey.ArticleTypeface);
    }

    public int FontSize
    {
      get => userSettings.Get<int>(UserSettingsKey.ArticleFontSize);
    }

    public bool StartHome
    {
      get => userSettings.Get<bool>(UserSettingsKey.StartHome);
    }

    public bool DisplayActive
    {
      get => userSettings.Get<bool>(UserSettingsKey.DisplayActive);
    }

    public Settings(IUserSettings userSettings)
    {
      Current = this;

      this.userSettings = userSettings;
      this.userSettings.SettingSet += (sender, settingKey) =>
      {
        if (settingKey == UserSettingsKey.AppTheme)
          OnPropertyChanged(nameof(AppTheme));
        else if (settingKey == UserSettingsKey.ArticleTypeface)
          OnPropertyChanged(nameof(Typeface));
        else if (settingKey == UserSettingsKey.ArticleFontSize)
          OnPropertyChanged(nameof(FontSize));
      };
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