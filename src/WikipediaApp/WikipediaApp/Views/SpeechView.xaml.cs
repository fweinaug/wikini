using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Media;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace WikipediaApp
{
  public sealed partial class SpeechView : UserControl
  {
    public static readonly DependencyProperty ArticleProperty = DependencyProperty.Register(
      "Article", typeof(Article), typeof(SpeechView), new PropertyMetadata(null, OnArticlePropertyChanged));

    private static readonly SpeechSynthesizer Synthesizer = new SpeechSynthesizer();

    private readonly SystemMediaTransportControls systemMediaControls;
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

      systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
      systemMediaControls.IsEnabled = false;
      systemMediaControls.ButtonPressed += SystemMediaControlsButtonPressed;

      systemMediaControls.IsPlayEnabled = true;
      systemMediaControls.IsPauseEnabled = true;
      systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;

      positionUpdateTimer.Interval = TimeSpan.FromMilliseconds(500);
      positionUpdateTimer.Tick += PositionUpdateTimerTick;

      UpdateSettings();

      SettingsVoiceComboBox.SelectionChanged += SettingsVoiceComboBoxSelectionChanged;
      SettingsSpeedSlider.ValueChanged += SettingsSpeedSliderValueChanged;
    }

    private async void SystemMediaControlsButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
    {
      switch (args.Button)
      {
        case SystemMediaTransportControlsButton.Play:
          await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Resume);
          break;
        case SystemMediaTransportControlsButton.Pause:
          await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Pause);
          break;
        case SystemMediaTransportControlsButton.Next:
          await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => GoForward());
          break;
        case SystemMediaTransportControlsButton.Previous:
          await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, GoBack);
          break;
      }
    }

    public void Unload()
    {
      systemMediaControls.IsEnabled = false;
      systemMediaControls.ButtonPressed -= SystemMediaControlsButtonPressed;

      positionUpdateTimer.Tick -= PositionUpdateTimerTick;

      SettingsVoiceComboBox.SelectionChanged -= SettingsVoiceComboBoxSelectionChanged;
      SettingsSpeedSlider.ValueChanged -= SettingsSpeedSliderValueChanged;
    }

    private void UpdateSettings()
    {
      var voices = SpeechSynthesizer.AllVoices;

      SettingsVoiceComboBox.ItemsSource = voices;
      SettingsVoiceComboBox.SelectedItem = voices.FirstOrDefault(x => x.Id == Synthesizer.Voice.Id);

      SettingsSpeedSlider.Value = Synthesizer.Options.SpeakingRate;
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

      systemMediaControls.IsEnabled = true;
      UpdateSystemMediaControlsInfo(article);
    }

    private void UpdateSystemMediaControlsInfo(Article article)
    {
      var updater = systemMediaControls.DisplayUpdater;

      updater.Type = MediaPlaybackType.Music;
      updater.MusicProperties.Artist = Package.Current.DisplayName;
      updater.MusicProperties.Title = article.Title;

      updater.Thumbnail = article.ThumbnailUri != null
        ? RandomAccessStreamReference.CreateFromUri(article.ThumbnailUri)
        : null;

      updater.Update();
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
      try
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
      }
      catch (Exception)
      {
        chapters.ResetActiveChapter();
      }
      finally
      {
        UpdatePosition();
      }
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

      switch (state)
      {
        case MediaElementState.Playing:
          systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Playing;
          break;
        case MediaElementState.Paused:
          systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Paused;
          break;
        case MediaElementState.Stopped:
          systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
          break;
        case MediaElementState.Closed:
          systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
          break;
      }

      PlayButton.IsEnabled = systemMediaControls.IsPlayEnabled = chapters.Count > 0;
      BackButton.IsEnabled = systemMediaControls.IsPreviousEnabled = currentChapterIndex > 0;
      NextButton.IsEnabled = systemMediaControls.IsNextEnabled = currentChapterIndex + 1 < chapters.Count;

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

    private void SettingsSpeedSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
      Synthesizer.Options.SpeakingRate = e.NewValue;

      TimeSpan GetAdjustedPosition()
      {
        var diff = e.OldValue / e.NewValue;
        var seconds = MediaElement.Position.TotalSeconds * diff;

        var position = TimeSpan.FromSeconds(seconds);

        return position;
      }

      ReloadCurrentChapter(GetAdjustedPosition);
    }

    private void SettingsVoiceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var voice = SettingsVoiceComboBox.SelectedItem as VoiceInformation;
      if (voice == null)
        return;

      Synthesizer.Voice = voice;

      ReloadCurrentChapter(() => MediaElement.Position);
    }

    private void ReloadCurrentChapter(Func<TimeSpan> positionFunc)
    {
      if (currentChapterIndex < 0)
        return;

      var state = MediaElement.CurrentState;
      if (state == MediaElementState.Closed || state == MediaElementState.Stopped)
        return;

      var position = positionFunc();

      if (position > TimeSpan.Zero)
      {
        void SetPositionAfterMediaOpened(object o, RoutedEventArgs args)
        {
          MediaElement.MediaOpened -= SetPositionAfterMediaOpened;

          MediaElement.Position = position;
        }

        MediaElement.MediaOpened += SetPositionAfterMediaOpened;
      }

      PlayChapter(currentChapterIndex);
    }
  }
}