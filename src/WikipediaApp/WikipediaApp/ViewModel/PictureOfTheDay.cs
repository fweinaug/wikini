using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikipediaApp
{
  public class PictureOfTheDay : ViewModelBase
  {
    private readonly WikipediaService wikipediaService;

    private DateTime date;
    private Uri thumbnailUri;

    private Command backCommand = null;
    private Command todayCommand = null;
    private Command randomCommand = null;
    private Command clearCommand = null;

    public PictureOfTheDay(WikipediaService wikipediaService)
    {
      this.wikipediaService = wikipediaService;
    }

    public DateTime Date
    {
      get { return date; }
      private set { SetProperty(ref date, value); }
    }

    public Uri ThumbnailUri
    {
      get { return thumbnailUri; }
      private set { SetProperty(ref thumbnailUri, value); }
    }

    public ICommand BackCommand
    {
      get { return backCommand ?? (backCommand = new Command(Back)); }
    }

    public ICommand TodayCommand
    {
      get { return todayCommand ?? (todayCommand = new Command(Today)); }
    }

    public ICommand RandomCommand
    {
      get { return randomCommand ?? (randomCommand = new Command(Random)); }
    }

    public ICommand ClearCommand
    {
      get { return clearCommand ?? (clearCommand = new Command(Clear)); }
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
    }

    private async Task ChangeDate(DateTime date)
    {
      var picture = await wikipediaService.GetPictureOfTheDay(date);

      ThumbnailUri = picture?.ThumbnailUri;
      Date = date;
    }
  }
}