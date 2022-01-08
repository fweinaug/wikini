namespace WikipediaApp
{
  public class ArticleLanguageViewModel : LanguageViewModel
  {
    public string LocalizedName { get; private set; }
    public string Title { get; private set; }

    public ArticleLanguageViewModel(ArticleLanguage language, int index) : base(language, index)
    {
      LocalizedName = language.LocalizedName;
      Title = language.Title;
    }
  }
}