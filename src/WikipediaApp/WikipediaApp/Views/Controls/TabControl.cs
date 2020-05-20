using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public sealed class TabControl : ItemsControl
  {
    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
      nameof(SelectedIndex), typeof(int), typeof(TabControl), new PropertyMetadata(-1, OnSelectedIndexChanged));

    public int SelectedIndex
    {
      get { return (int)GetValue(SelectedIndexProperty); }
      set { SetValue(SelectedIndexProperty, value); }
    }

    public TabControl()
    {
      DefaultStyleKey = typeof(TabControl);
    }

    private void UpdateSelectedButton(int oldIndex, int newIndex)
    {
      if (oldIndex >= 0 && oldIndex < Items.Count)
      {
        var button = (TabButton)Items[oldIndex];
        button.Selected = false;
      }

      if (newIndex >= 0 && newIndex < Items.Count)
      {
        var button = (TabButton)Items[newIndex];
        button.Selected = true;
      }
    }

    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var tabs = (TabControl)d;
      tabs.UpdateSelectedButton((int)e.OldValue, (int)e.NewValue);
    }
  }

  public sealed class TabButton : Control
  {
    public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(
      nameof(Glyph), typeof(string), typeof(TabButton), new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
      nameof(Selected), typeof(bool), typeof(TabButton), new PropertyMetadata(false, OnSelectedChanged));

    public string Glyph
    {
      get { return (string)GetValue(GlyphProperty); }
      set { SetValue(GlyphProperty, value); }
    }

    public bool Selected
    {
      get { return (bool)GetValue(SelectedProperty); }
      set { SetValue(SelectedProperty, value); }
    }

    public event RoutedEventHandler Click;

    public TabButton()
    {
      DefaultStyleKey = typeof(TabButton);
    }

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      var button = (Button)GetTemplateChild("PART_Button");
      button.Click += RaiseClick;
    }

    private void RaiseClick(object sender, RoutedEventArgs e)
    {
      if (!Selected)
      {
        Click?.Invoke(sender, e);
      }
    }

    private void UpdateState()
    {
      VisualStateManager.GoToState(this, Selected ? "Selected" : "Normal", true);
    }

    private static void OnSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var tabButton = (TabButton)d;
      tabButton.UpdateState();
    }
  }
}
