using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace WikipediaApp
{
  public class ArticleChapterCollection : ObservableCollection<ArticleChapterViewModel>
  {
    public bool ChapterIsBeingLoaded()
    {
      return Items.Any(item => item.IsLoading);
    }

    public void SetActiveChapter(int index)
    {
      for (var i = 0; i < Count; ++i)
      {
        var item = Items[i];
        var active = i == index;

        item.IsActive = item.IsLoading = active;
      }
    }

    public void ResetActiveChapter()
    {
      foreach (var item in Items)
      {
        item.IsActive = item.IsLoading = false;
      }
    }

    public void SetPlayingChapter(int index)
    {
      for (var i = 0; i < Count; ++i)
      {
        var item = Items[i];
        var playing = i == index;

        item.IsPlaying = playing;
        item.IsLoading = false;
      }
    }

    public void ResetPlayingChapter()
    {
      foreach (var item in Items)
      {
        item.IsPlaying = false;
      }
    }

    public void Update(Article article)
    {
      ClearItems();

      if (article != null)
      {
        var sections = ParseSections(article);
        AddItemsFromSections(sections, article.Title);
      }
    }

    private static Dictionary<HtmlNode, List<HtmlNode>> ParseSections(Article article)
    {
      var sections = new Dictionary<HtmlNode, List<HtmlNode>>();

      if (string.IsNullOrEmpty(article?.Content))
        return sections;

      var document = new HtmlDocument();
      document.LoadHtml(article.Content);

      RemoveUnwantedNodes(document);

      var rootNode = document.DocumentNode.Descendants("div").Single(x => x.HasClass("mw-parser-output"));

      foreach (var childNode in rootNode.ChildNodes)
      {
        if (!childNode.GetClasses().Any(c => c.StartsWith("mf-section-")))
          continue;

        var headlineNode = childNode.PreviousSibling?.OriginalName == "h2" ? childNode.PreviousSibling : rootNode;
        var nodes = new List<HtmlNode>();

        sections.Add(headlineNode, nodes);

        foreach (var grandchildNode in childNode.ChildNodes)
        {
          if (grandchildNode.OriginalName == "h3" || grandchildNode.OriginalName == "h4")
          {
            nodes = new List<HtmlNode>();

            sections.Add(grandchildNode, nodes);
          }
          else if (grandchildNode.OriginalName == "h5" || grandchildNode.OriginalName == "p" ||
                   grandchildNode.OriginalName == "ul" || grandchildNode.OriginalName == "ol" || grandchildNode.OriginalName == "dl" ||
                   grandchildNode.OriginalName == "table")
          {
            nodes.Add(grandchildNode);
          }
        }
      }

      return sections;
    }

    private static void RemoveUnwantedNodes(HtmlDocument document)
    {
      var rootNode = document.DocumentNode;

      var references = rootNode.Descendants("sup").Where(x => x.HasClass("reference") || x.HasClass("noprint")).ToList();
      references.ForEach(x => x.Remove());

      var ipas = rootNode.Descendants("span").Where(x => x.HasClass("IPA")).ToList();
      ipas.ForEach(x => x.Remove());

      var frames = rootNode.Descendants("div").Where(x => x.HasClass("NavFrame")).ToList();
      frames.ForEach(x => x.Remove());

      var styles = rootNode.Descendants("style").ToList();
      styles.ForEach(x => x.Remove());

      var annotations = rootNode.Descendants("annotation").ToList();
      annotations.ForEach(x => x.Remove());

      var infoboxes = rootNode.Descendants("table").Where(x => x.HasClass("infobox")).ToList();
      infoboxes.ForEach(x => x.Remove());

      var roles = rootNode.Descendants().Where(x =>
      {
        var role = x.GetAttributeValue("role", string.Empty);
        return role == "navigation" || role == "note";
      }).ToList();
      roles.ForEach(x => x.Remove());

      var hidden = rootNode.Descendants().Where(x =>
      {
        var style = x.GetAttributeValue("style", string.Empty).Replace(" ", "").Trim(';');
        return style == "visibility:hidden";
      }).ToList();
      hidden.ForEach(x => x.Remove());
    }

    private void AddItemsFromSections(Dictionary<HtmlNode, List<HtmlNode>> expandedSections, string title)
    {
      var mainNumber = 0;
      var extensionNumber = 0;
      var subNumber = 0;

      foreach (var section in expandedSections)
      {
        var chapter = new ArticleChapterViewModel();

        var headline = section.Key;
        if (headline.OriginalName == "h2")
        {
          mainNumber++;
          extensionNumber = 0;
          subNumber = 0;

          chapter.Headline = WikipediaApi.RemoveHtml(headline.InnerText);
          chapter.Level = 1;
          chapter.Number = mainNumber.ToString();
          chapter.Anchor = headline.LastChild.Id;
        }
        else if (headline.OriginalName == "h3")
        {
          extensionNumber++;
          subNumber = 0;

          chapter.Headline = WikipediaApi.RemoveHtml(headline.InnerText);
          chapter.Level = 2;
          chapter.Number = $"{mainNumber}.{extensionNumber}";
          chapter.Anchor = headline.LastChild.Id;
        }
        else if (headline.OriginalName == "h4")
        {
          subNumber++;

          chapter.Headline = WikipediaApi.RemoveHtml(headline.InnerText);
          chapter.Level = 3;
          chapter.Number = $"{mainNumber}.{extensionNumber}.{subNumber}";
          chapter.Anchor = headline.LastChild.Id;
        }
        else
        {
          chapter.Headline = title;
          chapter.Level = 1;
          chapter.Anchor = "section_0";
        }

        var contentBuilder = new StringBuilder();

        foreach (var child in section.Value)
        {
          var lines = child.InnerText.Split('\n');

          foreach (var line in lines)
          {
            var trimmedLine = line.Trim(' ', '\r');

            if (trimmedLine.Length > 0)
              contentBuilder.AppendLine(trimmedLine);
          }
        }

        chapter.Content = contentBuilder.ToString();

        Add(chapter);
      }
    }
  }
}