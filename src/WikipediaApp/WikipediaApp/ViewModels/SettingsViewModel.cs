using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace WikipediaApp
{
  public class SettingsViewModel : ObservableObject
  {
    private readonly IUserSettings userSettings;

    private RelayCommand clearHistoryCommand = null;

    public bool AppThemeSystem
    {
      get => userSettings.Get<int>(UserSettingsKey.AppTheme) == 0;
      set
      {
        if (!value)
          return;

        userSettings.Set(UserSettingsKey.AppTheme, 0);
      }
    }

    public bool AppThemeLight
    {
      get => userSettings.Get<int>(UserSettingsKey.AppTheme) == 1;
      set
      {
        if (!value)
          return;

        userSettings.Set(UserSettingsKey.AppTheme, 1);
      }
    }

    public bool AppThemeDark
    {
      get => userSettings.Get<int>(UserSettingsKey.AppTheme) == 2;
      set
      {
        if (!value)
          return;

        userSettings.Set(UserSettingsKey.AppTheme, 2);
      }
    }

    public bool SearchRestricted
    {
      get => userSettings.Get<bool>(UserSettingsKey.SearchRestricted);
      set => userSettings.Set(UserSettingsKey.SearchRestricted, value);
    }

    public bool SectionsCollapsed
    {
      get => userSettings.Get<bool>(UserSettingsKey.SectionsCollapsed);
      set => userSettings.Set(UserSettingsKey.SectionsCollapsed, value);
    }

    public bool ImagesDisabled
    {
      get => userSettings.Get<bool>(UserSettingsKey.ImagesDisabled);
      set => userSettings.Set(UserSettingsKey.ImagesDisabled, value);
    }

    public bool SplitViewInline
    {
      get => userSettings.Get<bool>(UserSettingsKey.SplitViewInline);
      set => userSettings.Set(UserSettingsKey.SplitViewInline, value);
    }

    public bool HistorySession
    {
      get => userSettings.Get<bool>(UserSettingsKey.HistorySession);
      set => userSettings.Set(UserSettingsKey.HistorySession, value);
    }

    public bool HistoryTimeline
    {
      get => userSettings.Get<bool>(UserSettingsKey.HistoryTimeline);
      set => userSettings.Set(UserSettingsKey.HistoryTimeline, value);
    }

    public bool TypefaceSerif
    {
      get => userSettings.Get<Typeface>(UserSettingsKey.ArticleTypeface) == Typeface.Serif;
      set
      {
        if (!value)
          return;

        userSettings.Set(UserSettingsKey.ArticleTypeface, Typeface.Serif);
      }
    }

    public bool TypefaceSansSerif
    {
      get => userSettings.Get<Typeface>(UserSettingsKey.ArticleTypeface) == Typeface.SansSerif;
      set
      {
        if (!value)
          return;

        userSettings.Set(UserSettingsKey.ArticleTypeface, Typeface.SansSerif);
      }
    }

    public int FontSize
    {
      get => userSettings.Get<int>(UserSettingsKey.ArticleFontSize);
      set => userSettings.Set(UserSettingsKey.ArticleFontSize, value);
    }

    public bool StartHome
    {
      get => userSettings.Get<bool>(UserSettingsKey.StartHome);
      set => userSettings.Set(UserSettingsKey.StartHome, value);
    }

    public bool StartPictureOfTheDay
    {
      get => userSettings.Get<bool>(UserSettingsKey.StartPictureOfTheDay);
      set => userSettings.Set(UserSettingsKey.StartPictureOfTheDay, value);
    }

    public bool DisplayActive
    {
      get => userSettings.Get<bool>(UserSettingsKey.DisplayActive);
      set => userSettings.Set(UserSettingsKey.DisplayActive, value);
    }

    public ICommand ClearHistoryCommand => clearHistoryCommand ??= new RelayCommand(ClearHistory, CanClearHistory);

    public SettingsViewModel(IUserSettings userSettings)
    {
      this.userSettings = userSettings;

      WeakReferenceMessenger.Default.Register<SettingsViewModel, HistoryChanged>(this, (_, message) =>
      {
        clearHistoryCommand?.NotifyCanExecuteChanged();
      });
    }

    private void ClearHistory()
    {
      WeakReferenceMessenger.Default.Send(new ClearHistory());
    }

    private bool CanClearHistory()
    {
      return !WeakReferenceMessenger.Default.Send(new IsHistoryEmpty());
    }
  }
}