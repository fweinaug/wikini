namespace WikipediaApp
{
  public class ArticleLanguageViewModel : LanguageViewModel
  {
    public string Title { get; private set; }

    public ArticleLanguageViewModel(ArticleLanguage language, int index) : base(language, index)
    {
      Title = language.Title;
    }
  }
}