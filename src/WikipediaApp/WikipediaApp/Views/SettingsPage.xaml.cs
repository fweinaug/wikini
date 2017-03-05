using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WikipediaApp
{
  public sealed partial class SettingsPage : Page
  {
    public SettingsPage()
    {
      InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      var package = Package.Current;
      var version = package.Id.Version;

      AppNameTextBlock.Text = package.DisplayName;
      AppVersionTextBlock.Text = string.Format("Version {0}.{1}.{2}", version.Major, version.Minor, version.Build);
      DevNameTextBlock.Text = package.PublisherDisplayName;

      var theme = Settings.Current.AppTheme.ToString();

      foreach (var uiElement in ThemePanel.Children)
      {
        var radioButton = uiElement as RadioButton;
        if (radioButton != null)
          radioButton.IsChecked = Equals(radioButton.Tag, theme);
      }

#if DEBUG
      await InAppPurchases.ConfigureSimulator();
#endif
    }

    private void RadioButtonThemeChecked(object sender, RoutedEventArgs e)
    {
      var radioButton = (RadioButton)sender;

      Settings.Current.AppTheme = Convert.ToInt32(radioButton.Tag);
    }

    private async void ReviewClick(object sender, RoutedEventArgs e)
    {
      var uri = new Uri("ms-windows-store://review/?ProductId=9nkvw50h59sl");
      await Launcher.LaunchUriAsync(uri);
    }

    private async void FeedbackClick(object sender, RoutedEventArgs e)
    {
      var uri = new Uri("mailto:wikiniapp@outlook.com");
      await Launcher.LaunchUriAsync(uri);
    }

    private async void DonateClick(object sender, RoutedEventArgs e)
    {
      var dialog = (ContentDialog)FindName("DonateContentDialog");
      if (dialog != null)
        await dialog.ShowAsync();
    }

    private void DonateOneButtonClick(object sender, RoutedEventArgs e)
    {
      Donate(InAppPurchases.Donation1);
    }

    private void DonateTwoButtonClick(object sender, RoutedEventArgs e)
    {
      Donate(InAppPurchases.Donation2);
    }

    private void DonateThreeButtonClick(object sender, RoutedEventArgs e)
    {
      Donate(InAppPurchases.Donation3);
    }

    private async void Donate(Func<Task<bool>> donateFunc)
    {
      var success = await donateFunc();
      if (success)
      {
        Settings.Current.ShowThanks = true;

        DonateContentDialog.Hide();
      }
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
  }
}