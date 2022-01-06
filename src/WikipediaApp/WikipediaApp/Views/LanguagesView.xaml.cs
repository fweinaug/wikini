using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public sealed partial class LanguagesView : UserControl
  {
    public event EventHandler LanguageClick;

    public static readonly DependencyProperty LanguagesProperty = DependencyProperty.Register(
      nameof(Languages), typeof(LanguageCollection), typeof(LanguagesView), new PropertyMetadata(null, OnLanguagesPropertyChanged));

    public static readonly DependencyProperty ChangeLanguageCommandProperty = DependencyProperty.Register(
      nameof(ChangeLanguageCommand), typeof(ICommand), typeof(LanguagesView), new PropertyMetadata(null, OnChangeLanguageCommandPropertyChanged));

    public LanguageCollection Languages
    {
      get { return (LanguageCollection)GetValue(LanguagesProperty); }
      set { SetValue(LanguagesProperty, value); }
    }

    public ICommand ChangeLanguageCommand
    {
      get { return (ICommand)GetValue(ChangeLanguageCommandProperty); }
      set { SetValue(ChangeLanguageCommandProperty, value); }
    }

    public LanguagesView()
    {
      InitializeComponent();

      LanguagesListView.ItemTemplateSelector = new LanguagesListViewItemTemplateSelector(this);
    }

    private void OnLanguagesListViewItemClick(object sender, ItemClickEventArgs e)
    {
      LanguageClick?.Invoke(this, EventArgs.Empty);
    }

    private static void OnLanguagesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (LanguagesView)d;
      control.LanguagesSource.Source = e.NewValue;
    }

    private static void OnChangeLanguageCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (LanguagesView)d;
      control.LanguagesListView.SetCommand(e.NewValue as ICommand);
    }

    private class LanguagesListViewItemTemplateSelector : DataTemplateSelector
    {
      private readonly LanguagesView view;

      public LanguagesListViewItemTemplateSelector(LanguagesView view)
      {
        this.view = view;
      }

      protected override DataTemplate SelectTemplateCore(object item)
      {
        switch (item)
        {
          case EmptyFavoritesHint _:
            {
              var template = (DataTemplate)view.Resources["EmptyFavoritesHintTemplate"];
              return template;
            }
          case ArticleLanguageViewModel _:
            {
              var template = (DataTemplate)view.Resources["ArticleLanguageTemplate"];
              return template;
            }
          default:
            {
              var template = (DataTemplate)view.Resources["LanguageTemplate"];
              return template;
            }
        }
      }
    }
  }

  public class LanguagesListView : GroupedListView
  {
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);

      if (item is EmptyFavoritesHint && element is ListViewItem listViewItem)
      {
        listViewItem.IsEnabled = false;
        listViewItem.IsHitTestVisible = false;
      }
    }
  }
}