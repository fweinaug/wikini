using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace WikipediaApp
{
  public sealed partial class ImageLoader : UserControl
  {
    public static readonly DependencyProperty UriProperty = DependencyProperty.Register(
      nameof(Uri), typeof(Uri), typeof(ImageLoader), new PropertyMetadata(null, OnUriPropertyChanged));

    public Uri Uri
    {
      get { return (Uri)GetValue(UriProperty); }
      set { SetValue(UriProperty, value); }
    }

    public ImageLoader()
    {
      InitializeComponent();
    }

    private static void OnUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (ImageLoader)d;

      control.ChangeImage(e.NewValue as Uri);
    }

    private void ChangeImage(Uri uri)
    {
      ProgressRing.Visibility = Visibility.Visible;
      ProgressRing.IsActive = true;

      Image.Source = new BitmapImage { UriSource = uri };
    }

    private void ImageOpened(object sender, RoutedEventArgs e)
    {
      ProgressRing.Visibility = Visibility.Collapsed;
      ProgressRing.IsActive = false;
    }

    private void ImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
      ProgressRing.Visibility = Visibility.Collapsed;
      ProgressRing.IsActive = false;
    }
  }
}
