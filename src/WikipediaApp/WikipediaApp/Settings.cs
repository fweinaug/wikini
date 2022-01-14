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
  }
}