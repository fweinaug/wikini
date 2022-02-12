using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace WikipediaApp
{
  public sealed partial class PhotoDescription : UserControl
  {
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
      nameof(Description), typeof(Description), typeof(PhotoDescription), new PropertyMetadata(null, OnDescriptionPropertyChanged));

    public static readonly DependencyProperty ArticleClickedCommandProperty = DependencyProperty.Register(
      nameof(ArticleClickedCommand), typeof(ICommand), typeof(PhotoDescription), new PropertyMetadata(null));

    public Description Description
    {
      get { return (Description)GetValue(DescriptionProperty); }
      set { SetValue(DescriptionProperty, value); }
    }

    public ICommand ArticleClickedCommand
    {
      get { return (ICommand)GetValue(ArticleClickedCommandProperty); }
      set { SetValue(ArticleClickedCommandProperty, value); }
    }

    public PhotoDescription()
    {
      InitializeComponent();
    }

    private static void OnDescriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (PhotoDescription)d;
      control.UpdateDescription();
    }

    private void UpdateDescription()
    {
      TextBlock.Blocks.Clear();

      if (!string.IsNullOrEmpty(Description?.Text))
      {
        var paragraph = BuildParagraph(Description.Language, Description.Text);

        TextBlock.Blocks.Add(paragraph);
      }
    }

    private Paragraph BuildParagraph(string language, string description)
    {
      var textRegex = new Regex(@"(('')?((\[\[(((w:)|:)?(?<bracketCategory>.+?):)?(?<bracketTitle>.+?)(\|(?<bracketText>.+?))?\]\])|(\{\{.*\|(?<braceText>.+?)\}\}))('')?)", RegexOptions.Singleline);
      var matches = textRegex.Matches(description);

      var paragraph = new Paragraph();

      var descriptionIndex = 0;
      foreach (Match match in matches)
      {
        var descriptionStart = description.Substring(descriptionIndex, match.Index - descriptionIndex);
        if (descriptionStart.Length > 0)
          AddText(paragraph.Inlines, descriptionStart);

        var bracketCategory = match.Groups["bracketCategory"].Value;
        var bracketText = match.Groups["bracketText"].Value.Trim();
        var bracketTitleMatch = match.Groups["bracketTitle"];
        var braceTextMatch = match.Groups["braceText"];

        if ((!string.IsNullOrEmpty(bracketCategory) && bracketCategory != "Category") || braceTextMatch.Success || bracketTitleMatch.Success)
        {
          var quote = match.Value.StartsWith("''") && match.Value.EndsWith("''");

          string title, linkText;
          if (braceTextMatch.Success)
          {
            var braceText = braceTextMatch.Value;

            if (braceTextMatch.Value.IndexOf('|') > 0)
            {
              title = braceText.Substring(0, braceText.IndexOf('|'));
              linkText = braceText.Substring(braceText.IndexOf('|') + 1);
            }
            else
            {
              title = braceText;
              linkText = braceText;
            }
          }
          else if (bracketTitleMatch.Success)
          {
            title = bracketTitleMatch.Value;
            linkText = !string.IsNullOrEmpty(bracketText) ? bracketText : title;
          }
          else
          {
            title = bracketText;
            linkText = bracketText;
          }

          var hyperlink = new Hyperlink();
          hyperlink.Click += (sender, e) =>
          {
            ArticleClickedCommand?.Execute(new Uri($"https://{(bracketCategory.Length > 0 ? bracketCategory : language)}.wikipedia.org/wiki/{(title)}"));
          };

          if (quote)
            hyperlink.FontStyle = FontStyle.Italic;

          AddText(hyperlink.Inlines, linkText);

          paragraph.Inlines.Add(hyperlink);
        }
        else
        {
          AddText(paragraph.Inlines, bracketText);
        }

        descriptionIndex = match.Index + match.Length;
      }

      var descriptionEnd = description.Substring(descriptionIndex);
      if (descriptionEnd.Length > 0)
        AddText(paragraph.Inlines, descriptionEnd);
      
      return paragraph;
    }

    private static void AddText(InlineCollection inlines, string text)
    {
      var parts = text.Split("''");

      for (var i = 0; i < parts.Length; i++)
      {
        var part = parts[i];
        if (part.Length == 0)
          continue;

        var run = new Run { Text = part };
        if (i % 2 != 0)
          run.FontStyle = FontStyle.Italic;

        inlines.Add(run);
      }
    }
  }
}