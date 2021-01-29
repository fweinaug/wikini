using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace WikipediaApp
{
  public sealed partial class ArticleImagesView : UserControl
  {
    public static readonly DependencyProperty ImagesProperty = DependencyProperty.Register(
      nameof(Images), typeof(IEnumerable<ArticleImage>), typeof(ArticleImagesView), new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedImageProperty = DependencyProperty.Register(
      nameof(SelectedImage), typeof(ArticleImage), typeof(ArticleImagesView), new PropertyMetadata(null));

    public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(
      nameof(CloseCommand), typeof(ICommand), typeof(ArticleImagesView), new PropertyMetadata(null));

    public IEnumerable<ArticleImage> Images
    {
      get { return (IEnumerable<ArticleImage>)GetValue(ImagesProperty); }
      set { SetValue(ImagesProperty, value); }
    }

    public ArticleImage SelectedImage
    {
      get { return (ArticleImage)GetValue(SelectedImageProperty); }
      set { SetValue(SelectedImageProperty, value); }
    }

    public ICommand CloseCommand 
    {
      get { return (ICommand)GetValue(CloseCommandProperty); }
      set { SetValue(CloseCommandProperty, value); }
    }

    public ArticleImagesView()
    {
      InitializeComponent();
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
      if (e.Key == Windows.System.VirtualKey.Escape)
      {
        Close();

        e.Handled = true;
      }

      base.OnKeyDown(e);
    }

    private void CloseButtonClick(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void Close()
    {
      CloseCommand?.Execute(null);
    }

    private void FlipViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var position = FlipView.Items?.Count > 0
        ? string.Format("{0}/{1}", FlipView.SelectedIndex + 1, FlipView.Items.Count)
        : string.Empty;

      PositionTextBlock.Text = position;
    }

    private async void DownloadButtonClick(object sender, RoutedEventArgs e)
    {
      if (SelectedImage != null)
        await DownloadHelper.DownloadImage(SelectedImage);
    }
  }
}