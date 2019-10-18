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

    public async void Back()
    {
      if (date > DateTime.MinValue)
      {
        await ChangeDate(date.AddDays(-1));
      }
    }

    public async void Today()
    {
      var today = DateTime.Today;

      if (date != today)
      {
        await ChangeDate(today);
      }
    }

    private async Task ChangeDate(DateTime date)
    {
      var picture = await wikipediaService.GetPictureOfTheDay(date);

      ThumbnailUri = picture?.ThumbnailUri;
      Date = date;
    }
  }
}