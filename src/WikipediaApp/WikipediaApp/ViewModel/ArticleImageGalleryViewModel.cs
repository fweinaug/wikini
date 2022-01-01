using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace WikipediaApp
{
  public class ArticleImageGalleryViewModel : ObservableObject
  {
    private readonly IWikipediaService wikipediaService = new WikipediaService();

    private readonly Article article;

    private IList<ArticleImage> images = null;
    private ArticleImage selectedImage = null;
    private bool isOpen = false;

    private RelayCommand openCommand;
    private RelayCommand closeCommand;

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

    public bool IsOpen
    {
      get { return isOpen; }
      private set { SetProperty(ref isOpen, value); }
    }

    public ICommand OpenCommand
    {
      get { return openCommand ?? (openCommand = new RelayCommand(Open)); }
    }

    public ICommand CloseCommand
    {
      get { return closeCommand ?? (closeCommand = new RelayCommand(Close)); }
    }

    public ArticleImageGalleryViewModel(Article article)
    {
      this.article = article;
    }

    public async Task<bool> NavigateToImage(Uri uri)
    {
      if (article?.Images != null && wikipediaService.IsLinkToWikipediaImage(uri, out var filename)
        && article.Images.Contains(filename))
      {
        await LoadAndShowImages(filename);

        return true;
      }

      return false;
    }

    private async void Open()
    {
      await LoadAndShowImages();
    }

    private void Close()
    {
      IsOpen = false;
    }

    private async Task LoadAndShowImages(string selectedImageName = null)
    {
      IsOpen = true;

      if (Images == null)
        Images = await wikipediaService.GetArticleImages(article);

      SelectedImage = !string.IsNullOrEmpty(selectedImageName)
        ? Images?.FirstOrDefault(x => x.Name == selectedImageName)
        : Images?.FirstOrDefault();
    }
  }
}