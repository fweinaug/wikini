using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Globalization;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LanguageInfo = Windows.Globalization.Language;
using Microsoft.Extensions.DependencyInjection;

namespace WikipediaApp
{
  public sealed partial class SettingsView : UserControl
  {
    private const string LongVersionFormat = "Version {0}.{1}.{2}";
    private const string ShortVersionFormat = "Version {0}.{1}";

    public SettingsView()
    {
      InitializeComponent();

      DataContext = App.Services.GetService<SettingsViewModel>();

      var package = Package.Current;
      var version = package.Id.Version;

      AppNameTextBlock.Text = package.DisplayName;
      AppVersionTextBlock.Text = string.Format(version.Build > 0 ? LongVersionFormat : ShortVersionFormat, version.Major, version.Minor, version.Build);
      DevNameTextBlock.Text = package.PublisherDisplayName;

      LanguagesComboBox.ItemsSource = ApplicationLanguages.ManifestLanguages.Select(x => new LanguageInfo(x));
      LanguagesComboBox.SelectedValue = App.Services.GetService<IAppSettings>().Language;
    }

    public void CloseDialogs()
    {
      ChangelogContentDialog?.Hide();
      DisclaimerContentDialog?.Hide();
    }

    private void LanguagesComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (LanguagesComboBox.SelectedValue is string language)
        ApplicationLanguages.PrimaryLanguageOverride = language;
    }

    private async void ReviewClick(object sender, RoutedEventArgs e)
    {
      var uri = new Uri("ms-windows-store://review/?ProductId=9nkvw50h59sl");
      await Launcher.LaunchUriAsync(uri);
    }

    private async void FeedbackClick(object sender, RoutedEventArgs e)
    {
      var uri = new Uri("mailto:fweinaug-apps@outlook.com?subject=Wikini");
      await Launcher.LaunchUriAsync(uri);
    }

    private async void DisclaimerClick(object sender, RoutedEventArgs e)
    {
      var dialog = (ContentDialog)FindName("DisclaimerContentDialog");
      if (dialog != null)
        await dialog.ShowAsync();
    }

    private async void ChangelogClick(object sender, RoutedEventArgs e)
    {
      var dialog = (ContentDialog)FindName("ChangelogContentDialog");
      if (dialog != null)
      {
        await ChangelogView.LoadAndShowChangelog();
        await dialog.ShowAsync();
      }
    }

    private async void WebsiteClick(object sender, RoutedEventArgs e)
    {
      var uri = new Uri("http://wikiniapp.com");
      await Launcher.LaunchUriAsync(uri);
    }
  }
}