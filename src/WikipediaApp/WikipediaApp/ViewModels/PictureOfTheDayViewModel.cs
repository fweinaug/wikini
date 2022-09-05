using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WikipediaApp
{
  public class PictureOfTheDayViewModel : ObservableObject
  {
    private readonly IWikipediaService wikipediaService;
    private readonly IAppSettings appSettings;

    private DateTime date;
    private Uri thumbnailUri;
    private Description description;

    private RelayCommand backCommand = null;
    private RelayCommand todayCommand = null;
    private RelayCommand randomCommand = null;
    private RelayCommand clearCommand = null;

    public DateTime Date
    {
      get { return date; }
      private set { SetProperty(ref date, value); }
    }

    public Uri ThumbnailUri
    {
      get { return thumbnailUri; }
      private set
      {
        if (SetProperty(ref thumbnailUri, value))
          clearCommand?.NotifyCanExecuteChanged();
      }
    }

    public Description Description
    {
      get { return description; }
      private set
      {
        if (SetProperty(ref description, value))
          OnPropertyChanged(nameof(HasDescription));
      }
    }

    public bool HasDescription => Description != null;

    public ICommand BackCommand
    {
      get { return backCommand ?? (backCommand = new RelayCommand(Back)); }
    }

    public ICommand TodayCommand
    {
      get { return todayCommand ?? (todayCommand = new RelayCommand(Today)); }
    }

    public ICommand RandomCommand
    {
      get { return randomCommand ?? (randomCommand = new RelayCommand(Random)); }
    }

    public ICommand ClearCommand
    {
      get { return clearCommand ?? (clearCommand = new RelayCommand(Clear, () => thumbnailUri != null)); }
    }

    public PictureOfTheDayViewModel(IWikipediaService wikipediaService, IAppSettings appSettings)
    {
      this.wikipediaService = wikipediaService;
      this.appSettings = appSettings;
    }

    public async void Back()
    {
      var previousDate = date > DateTime.MinValue ? date.AddDays(-1) : DateTime.Today;

      await ChangeDate(previousDate);
    }

    public async void Today()
    {
      var today = DateTime.Today;

      if (thumbnailUri == null || date != today)
      {
        await ChangeDate(today);
      }
    }

    public async void Random()
    {
      DateTime randomDate;
      var random = new Random();

      do
      {
        var days = random.Next(1, 365);
        randomDate = DateTime.Today.AddDays(-days);
      } while (randomDate == date);

      await ChangeDate(randomDate);
    }

    public void Clear()
    {
      ThumbnailUri = null;
      Description = null;
    }

    private async Task ChangeDate(DateTime date)
    {
      var picture = await wikipediaService.GetPictureOfTheDay(date);

      ThumbnailUri = picture?.ThumbnailUri;
      Description = picture?.GetDescription(appSettings.Language);
      Date = date;
    }
  }
}