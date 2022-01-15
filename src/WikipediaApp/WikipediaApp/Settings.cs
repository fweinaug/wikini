using Microsoft.Toolkit.Mvvm.ComponentModel;

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

    public Typeface Typeface
    {
      get => userSettings.Get<Typeface>(UserSettingsKey.ArticleTypeface);
    }

    public int FontSize
    {
      get => userSettings.Get<int>(UserSettingsKey.ArticleFontSize);
    }

    public Settings(IUserSettings userSettings)
    {
      Current = this;

      this.userSettings = userSettings;
      this.userSettings.SettingSet += (sender, settingKey) =>
      {
        if (settingKey == UserSettingsKey.ArticleTypeface)
          OnPropertyChanged(nameof(Typeface));
        else if (settingKey == UserSettingsKey.ArticleFontSize)
          OnPropertyChanged(nameof(FontSize));
      };
    }
  }
}