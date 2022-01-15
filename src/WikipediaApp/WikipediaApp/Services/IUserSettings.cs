using System;
using System.Collections.Generic;

namespace WikipediaApp
{
  public interface IUserSettings
  {
    event EventHandler<string> SettingSet;

    void Set<T>(string settingKey, T value);

    T Get<T>(string settingKey);
    T Get<T>(string settingKey, T defaultValue);
  }

  public static class UserSettingsKey
  {
    public const string AppTheme = "AppTheme";
    public const string SearchLanguage = "SearchLanguage";
    public const string SearchRestricted = "SearchRestricted";
    public const string SectionsCollapsed = "SectionsCollapsed";
    public const string ImagesDisabled = "ImagesDisabled";
    public const string SplitViewInline = "SplitViewInline";
    public const string HistorySession = "HistorySession";
    public const string HistoryTimeline = "HistoryTimeline";
    public const string ArticleTypeface = "Typeface";
    public const string ArticleFontSize = "FontSize";
    public const string StartHome = "StartHome";
    public const string StartPictureOfTheDay = "StartPictureOfTheDay";
    public const string DisplayActive = "DisplayActive";
    public const string LastArticle = "LastArticle";

    public static IReadOnlyDictionary<string, object> Defaults { get; } = new Dictionary<string, object>
    {
      { AppTheme, 0 },
      { SearchLanguage, "en" },
      { SearchRestricted, false },
      { SectionsCollapsed, false },
      { ImagesDisabled, false },
      { SplitViewInline, false },
      { HistorySession, false },
      { HistoryTimeline, true },
      { ArticleTypeface, Typeface.SansSerif },
      { ArticleFontSize, 16 },
      { StartHome, false },
      { StartPictureOfTheDay, true },
      { DisplayActive, false },
      { LastArticle, null },
    };

    public static T Default<T>(string settingKey)
    {
      return (T)Defaults[settingKey];
    }
  }

  public enum Typeface
  {
    SansSerif,
    Serif
  }
}