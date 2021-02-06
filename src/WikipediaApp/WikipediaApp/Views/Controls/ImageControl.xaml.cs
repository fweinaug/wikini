using System;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace WikipediaApp
{
  public sealed partial class ImageControl : UserControl
  {
    public static readonly DependencyProperty UriProperty = DependencyProperty.Register(
      nameof(Uri), typeof(Uri), typeof(ImageControl), new PropertyMetadata(null, OnUriPropertyChanged));

    private PointerPoint pointerPressedAtPoint = null;
    private Point pointerPressedOffset;

    public Uri Uri
    {
      get { return (Uri)GetValue(UriProperty); }
      set { SetValue(UriProperty, value); }
    }

    public ImageControl()
    {
      InitializeComponent();
    }

    private static void OnUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (ImageControl)d;

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

    private void ScrollViewerDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
      if (ScrollViewer.ZoomFactor > 1)
      {
        ScrollViewer.ChangeView(null, null, 1);
      }
      else if (ScrollViewer.MaxZoomFactor > 1)
      {
        var point = e.GetPosition(Image);

        var horizontalOffset = point.X - ((ScrollViewer.ActualWidth - Image.ActualWidth) / 2);
        var verticalOffset = point.Y - ((ScrollViewer.ActualHeight - Image.ActualHeight) / 2);

        ScrollViewer.ChangeView(Math.Max(horizontalOffset, 0), Math.Max(verticalOffset, 0), 2);
      }

      ScrollViewer.ReleasePointerCaptures();
    }

    private void ScrollViewerPointerPressed(object sender, PointerRoutedEventArgs e)
    {
      if (ScrollViewer.ZoomFactor <= 1)
        return;

      var point = e.GetCurrentPoint(ScrollViewer);
      if (point.PointerDevice.PointerDeviceType == PointerDeviceType.Mouse && !point.Properties.IsLeftButtonPressed)
        return;

      pointerPressedAtPoint = point;
      pointerPressedOffset = new Point(ScrollViewer.HorizontalOffset, ScrollViewer.VerticalOffset);

      ScrollViewer.CapturePointer(e.Pointer);
    }

    private void ScrollViewerPointerReleased(object sender, PointerRoutedEventArgs e)
    {
      ScrollViewer.ReleasePointerCaptures();

      pointerPressedAtPoint = null;
    }

    private void ScrollViewerPointerMoved(object sender, PointerRoutedEventArgs e)
    {
      if (ScrollViewer.PointerCaptures == null || ScrollViewer.PointerCaptures.Count == 0)
        return;

      var point = e.GetCurrentPoint(ScrollViewer);
      var horizontalOffset = pointerPressedOffset.X + pointerPressedAtPoint.Position.X - point.Position.X;
      var verticalOffset = pointerPressedOffset.Y + pointerPressedAtPoint.Position.Y - point.Position.Y;

      ScrollViewer.ChangeView(horizontalOffset, verticalOffset, null, true);
    }
  }
}