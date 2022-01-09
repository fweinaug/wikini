using System;
using Windows.Storage;

namespace WikipediaApp
{
  public class RoamingUserSettings : IUserSettings
  {
    public event EventHandler<string> SettingSet;

    public T Get<T>(string settingKey)
    {
      return Get(settingKey, UserSettingsKey.Default<T>(settingKey));
    }

    public T Get<T>(string settingKey, T defaultValue)
    {
      var value = ApplicationData.Current.RoamingSettings.Values[settingKey] ?? defaultValue;

      return typeof(T).IsEnum
        ? (T)Enum.Parse(typeof(T), value.ToString())
        : (T)value;
    }

    public void Set<T>(string settingKey, T value)
    {
      ApplicationData.Current.RoamingSettings.Values[settingKey] = typeof(T).IsEnum ? value.ToString() : value;

      SettingSet?.Invoke(this, settingKey);
    }
  }
}