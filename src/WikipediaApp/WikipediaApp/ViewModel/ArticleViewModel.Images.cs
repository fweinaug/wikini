using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikipediaApp
{
  public partial class ArticleViewModel
  {
    private IList<ArticleImage> images = null;
    private ArticleImage selectedImage = null;
    private bool imagesVisible = false;

    private Command showImagesCommand;
    private Command hideImagesCommand;

    public IList<ArticleImage> Images
    {
      get { return images; }
      private set { SetProperty(ref images, value); }
    }

    public ArticleImage SelectedImage
    {
      get { return selectedImage; }
      set { SetProperty(ref selectedImage, value); }
    }

    public bool ImagesVisible
    {
      get { return imagesVisible; }
      private set { SetProperty(ref imagesVisible, value); }
    }

    public ICommand ShowImagesCommand
    {
      get { return showImagesCommand ?? (showImagesCommand = new Command(ShowImages)); }
    }

    public ICommand HideImagesCommand
    {
      get { return hideImagesCommand ?? (hideImagesCommand = new Command(HideImages)); }
    }

    private async void ShowImages()
    {
      await LoadAndShowImages();
    }

    private void HideImages()
    {
      ImagesVisible = false;
    }

    private async Task<bool> NavigateToImage(Uri uri)
    {
      if (article?.Images != null && wikipediaService.IsLinkToWikipediaImage(uri, out var filename)
        && article.Images.Contains(filename))
      {
        await LoadAndShowImages(filename);

        return true;
      }

      return false;
    }

    private async Task LoadAndShowImages(string selectedImageName = null)
    {
      IsBusy = true;

      if (Images == null)
        Images = await wikipediaService.GetArticleImages(article);

      SelectedImage = !string.IsNullOrEmpty(selectedImageName)
        ? Images?.FirstOrDefault(x => x.Name == selectedImageName)
        : Images?.FirstOrDefault();

      IsBusy = false;
      ImagesVisible = true;
    }
  }
}