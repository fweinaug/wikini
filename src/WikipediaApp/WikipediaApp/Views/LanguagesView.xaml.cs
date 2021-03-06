﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WikipediaApp
{
  public sealed partial class LanguagesView : UserControl
  {
    public event EventHandler LanguageClick;

    public static readonly DependencyProperty LanguagesProperty = DependencyProperty.Register(
      nameof(Languages), typeof(IEnumerable<Language>), typeof(LanguagesView), new PropertyMetadata(null, OnLanguagesPropertyChanged));

    public IEnumerable<Language> Languages
    {
      get { return (IEnumerable<Language>)GetValue(LanguagesProperty); }
      set { SetValue(LanguagesProperty, value); }
    }

    private readonly LanguageCollection languageCollection = new LanguageCollection();

    public LanguagesView()
    {
      InitializeComponent();

      LanguagesListView.ItemTemplateSelector = new LanguagesListViewItemTemplateSelector(this);
      LanguagesSource.Source = languageCollection;
    }

    private void OnLanguagesListViewItemClick(object sender, ItemClickEventArgs e)
    {
      LanguageClick?.Invoke(this, EventArgs.Empty);
    }

    private void OnAddFavoriteMenuFlyoutItemClick(object sender, RoutedEventArgs e)
    {
      var item = (MenuFlyoutItem)sender;
      var language = (Language)item.DataContext;

      languageCollection.AddFavorite(language);
    }

    private void OnRemoveFavoriteMenuFlyoutItemClick(object sender, RoutedEventArgs e)
    {
      var item = (MenuFlyoutItem)sender;
      var language = (Language)item.DataContext;

      languageCollection.RemoveFavorite(language);
    }

    private void UpdateLanguages(IEnumerable<Language> languages)
    {
      languageCollection.UpdateLanguages(languages);
    }

    private static void OnLanguagesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (LanguagesView)d;
      
      control.UpdateLanguages(e.NewValue as IEnumerable<Language>);
    }

    private class LanguageGroup : Group<string, Language>
    {
    }

    private class LanguageCollection : ObservableCollection<LanguageGroup>
    {
      private readonly LanguageGroup favoritesGroup = new LanguageGroup { Key = "LanguageGroupFavorites" };
      private readonly LanguageGroup otherGroup = new LanguageGroup { Key = "LanguageGroupMore" };

      private static readonly EmptyFavoritesHint EmptyFavoritesHint = new EmptyFavoritesHint();

      public LanguageCollection()
      {
        Add(favoritesGroup);
        Add(otherGroup);
      }

      public void UpdateLanguages(IEnumerable<Language> languages)
      {
        favoritesGroup.Clear();
        otherGroup.Clear();

        if (languages != null)
        {
          foreach (var language in languages)
          {
            language.IsFavorite = ArticleLanguages.IsFavorite(language.Code);

            if (language.IsFavorite)
              favoritesGroup.Add(language);
            else
              otherGroup.Add(language);
          }
        }

        if (favoritesGroup.Count == 0)
          favoritesGroup.Add(EmptyFavoritesHint);
      }

      public void AddFavorite(Language language)
      {
        ArticleLanguages.AddFavorite(language);

        MoveLanguage(language, otherGroup, favoritesGroup);

        favoritesGroup.Remove(EmptyFavoritesHint);
      }

      public void RemoveFavorite(Language language)
      {
        ArticleLanguages.RemoveFavorite(language);

        MoveLanguage(language, favoritesGroup, otherGroup);

        if (favoritesGroup.Count == 0)
          favoritesGroup.Add(EmptyFavoritesHint);
      }

      private static void MoveLanguage(Language language, LanguageGroup from, LanguageGroup to)
      {
        var index = to.TakeWhile(x => language.Index > x.Index).Count();

        from.Remove(language);
        to.Insert(index, language);
      }
    }

    private class EmptyFavoritesHint : Language, IDisabledListViewItem
    {
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
          case ArticleLanguage _:
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
}