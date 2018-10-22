﻿using System;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace WikipediaApp
{
  public sealed partial class SpeechView : UserControl
  {
    public static readonly DependencyProperty ArticleProperty = DependencyProperty.RegisterAttached(
      "Article", typeof(Article), typeof(SpeechView), new PropertyMetadata(null, OnArticlePropertyChanged));

    private static readonly SpeechSynthesizer Synthesizer = new SpeechSynthesizer();

    private readonly DispatcherTimer positionUpdateTimer = new DispatcherTimer();
    private readonly ArticleChapterCollection chapters = new ArticleChapterCollection();
    private int currentChapterIndex = -1;

    public Article Article
    {
      get { return (Article)GetValue(ArticleProperty); }
      set { SetValue(ArticleProperty, value); }
    }

    public SpeechView()
    {
      InitializeComponent();

      ListView.ItemsSource = chapters;

      positionUpdateTimer.Interval = TimeSpan.FromMilliseconds(500);
      positionUpdateTimer.Tick += PositionUpdateTimerTick;
    }

    private static void OnArticlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var article = e.NewValue as Article;

      var view = (SpeechView)d;
      view.ChangeArticle(article);
    }

    private void ChangeArticle(Article article)
    {
      MediaElement.Stop();
      MediaElement.Source = null;

      currentChapterIndex = -1;
      chapters.Update(article);

      UpdateViewState();
    }

    private void PositionUpdateTimerTick(object sender, object e)
    {
      UpdatePosition();
    }

    private void PlayClick(object sender, RoutedEventArgs e)
    {
      Resume();
    }

    private void PauseClick(object sender, RoutedEventArgs e)
    {
      Pause();
    }

    private void BackClick(object sender, RoutedEventArgs e)
    {
      GoBack();
    }

    private void ForwardClick(object sender, RoutedEventArgs e)
    {
      GoForward();
    }

    private void ListViewItemClick(object sender, ItemClickEventArgs e)
    {
      var chapter = (ArticleChapter)e.ClickedItem;

      PlayOrPauseChapter(chapter);
    }

    private void MediaElementCurrentStateChanged(object sender, RoutedEventArgs e)
    {
      UpdateViewState();
    }

    private void MediaElementMediaEnded(object sender, RoutedEventArgs e)
    {
      if (chapters.ChapterIsBeingLoaded())
        return;

      GoForward(stopAtLastChapter: true);
    }

    private void Pause()
    {
      MediaElement.Pause();
    }

    private void Resume()
    {
      if (currentChapterIndex >= 0)
        MediaElement.Play();
      else
        PlayChapter(currentChapterIndex = 0);
    }

    private void Stop()
    {
      MediaElement.Stop();
      MediaElement.Source = null;

      currentChapterIndex = -1;
      chapters.ResetActiveChapter();
    }

    private void GoForward(bool stopAtLastChapter = false)
    {
      if (currentChapterIndex + 1 < chapters.Count)
        PlayChapter(++currentChapterIndex);
      else if (stopAtLastChapter)
        Stop();
    }

    private void GoBack()
    {
      if (currentChapterIndex > 0)
        PlayChapter(--currentChapterIndex);
    }

    private void PlayOrPauseChapter(ArticleChapter chapter)
    {
      if (chapter.IsPlaying)
        MediaElement.Pause();
      else if (chapter.IsActive)
        MediaElement.Play();
      else
        PlayChapter(currentChapterIndex = chapters.IndexOf(chapter));
    }

    private async void PlayChapter(int index)
    {
      chapters.SetActiveChapter(index);

      var chapter = chapters[index];

      var ssml =
        $@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='{Article.Language}'>
        {chapter.Headline}
        {chapter.Content}
        </speak>";

      var stream = await Synthesizer.SynthesizeSsmlToStreamAsync(ssml);

      MediaElement.SetSource(stream, stream.ContentType);

      UpdatePosition();
    }

    private void UpdateViewState()
    {
      var state = MediaElement.CurrentState;

      switch (state)
      {
        case MediaElementState.Opening:
        case MediaElementState.Playing:
          PlayButton.Visibility = Visibility.Collapsed;
          PauseButton.Visibility = Visibility.Visible;

          positionUpdateTimer.Start();
          break;
        case MediaElementState.Paused:
        case MediaElementState.Stopped:
        case MediaElementState.Closed:
          PlayButton.Visibility = Visibility.Visible;
          PauseButton.Visibility = Visibility.Collapsed;

          positionUpdateTimer.Stop();
          break;
      }

      PlayButton.IsEnabled = chapters.Count > 0;
      BackButton.IsEnabled = currentChapterIndex > 0;
      NextButton.IsEnabled = currentChapterIndex + 1 < chapters.Count;

      if (state == MediaElementState.Playing || state == MediaElementState.Closed)
        UpdatePosition();

      if (state == MediaElementState.Playing)
        chapters.SetPlayingChapter(currentChapterIndex);
      else
        chapters.ResetPlayingChapter();
    }

    private void UpdatePosition()
    {
      var position = MediaElement.Position;
      var duration = MediaElement.NaturalDuration.TimeSpan;

      PositionTextBlock.Text = $"{position:mm\\:ss} / {duration:mm\\:ss}";

      PositionProgressBar.Maximum = duration.TotalMilliseconds;
      PositionProgressBar.Value = position.TotalMilliseconds;
    }
  }
}