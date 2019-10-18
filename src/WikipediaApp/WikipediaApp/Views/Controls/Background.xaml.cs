using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace WikipediaApp
{
  public sealed partial class Background : UserControl
  {
    public static readonly DependencyProperty ImageUriProperty = DependencyProperty.Register(
      "ImageUri", typeof(Uri), typeof(Background), new PropertyMetadata(null, OnImageUriPropertyChanged));

    public Uri ImageUri
    {
      get { return (Uri)GetValue(ImageUriProperty); }
      set { SetValue(ImageUriProperty, value); }
    }

    public Background()
    {
      InitializeComponent();
    }

    private void UpdateImage()
    {
      HideImageStoryboard.Begin();
    }

    private void OnHideImageStoryboardCompleted(object sender, object e)
    {
      if (ImageUri != null)
      {
        ImageBrush.ImageSource = new BitmapImage { UriSource = ImageUri };
      }
      else
      {
        ImageBrush.ImageSource = null;
        OverlayRectangle.Visibility = Visibility.Collapsed;
      }
    }

    private void OnImageOpened(object sender, RoutedEventArgs e)
    {
      ShowImageStoryboard.Begin();

      OverlayRectangle.Visibility = Visibility.Visible;
    }

    private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
      OverlayRectangle.Visibility = Visibility.Collapsed;
    }

    private static void OnImageUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (Background)d;

      control.UpdateImage();
    }
  }
}