using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WikipediaApp
{
  public sealed partial class MapPage : Page
  {
    public MapPage()
    {
      InitializeComponent();

      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
      UpdateTitleBar(coreTitleBar);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
      coreTitleBar.LayoutMetricsChanged += TitleBarLayoutMetricsChanged;
      coreTitleBar.IsVisibleChanged += TitleBarIsVisibleChanged;
      
      DataContext = e.Parameter;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
      coreTitleBar.LayoutMetricsChanged -= TitleBarLayoutMetricsChanged;
      coreTitleBar.IsVisibleChanged -= TitleBarIsVisibleChanged;
    }

    private void TitleBarLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
    {
      UpdateTitleBar(sender);
    }

    private void TitleBarIsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
    {
      UpdateTitleBar(sender);
    }

    private void UpdateTitleBar(CoreApplicationViewTitleBar sender)
    {
      var height = sender.IsVisible && sender.Height > 0 ? sender.Height + 1 : 0;

      AppTitleBarBackground.Height = height;
    }
  }
}