using System;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace WikipediaApp
{
  public sealed partial class ChangelogView : UserControl
  {
    public ChangelogView()
    {
      InitializeComponent();
    }

    public async Task LoadAndShowChangelog()
    {
      if (ChangelogRichTextBlock.Blocks.Count > 0)
        return;

      var accentBrush = (Brush)Resources["SystemControlHighlightAccentBrush"];

      var versions = await ReadFile("ms-appx:///Data/versions.json");
      var changelog = await ReadFile("ms-appx:///Data/changelog.json");

      for (var i = 0; i < versions.Count; i++)
      {
        var value = versions[i];
        var obj = value.GetObject();

        var versionNumber = obj.GetNamedString("VersionNumber");
        var releaseDate = DateTime.Parse(obj.GetNamedString("ReleaseDate"));
        var changes = changelog[i].GetArray();

        var paragraph = new Paragraph { FontSize = 14, Margin = new Thickness(0, 15, 5, 0) };

        paragraph.Inlines.Add(new Run { Text = string.Format("Version {0}", versionNumber), FontWeight = FontWeights.Bold, Foreground = accentBrush });
        paragraph.Inlines.Add(new Run { Text = string.Format("  ({0:d})", releaseDate), FontWeight = FontWeights.Thin, FontSize = 12 });
        paragraph.Inlines.Add(new LineBreak());

        var grid = new Grid();

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition());

        foreach (var change in changes)
        {
          grid.RowDefinitions.Add(new RowDefinition());

          var bulletTextBlock = new TextBlock { Text = "\x2022", Margin = new Thickness(0, 0, 5, 0) };
          var changeTextBlock = new TextBlock { Text = change.GetString(), TextWrapping = TextWrapping.WrapWholeWords };

          grid.Children.Add(bulletTextBlock);
          grid.Children.Add(changeTextBlock);

          Grid.SetColumn(bulletTextBlock, 0);
          Grid.SetColumn(changeTextBlock, 1);

          Grid.SetRow(bulletTextBlock, grid.RowDefinitions.Count - 1);
          Grid.SetRow(changeTextBlock, grid.RowDefinitions.Count - 1);
        }

        paragraph.Inlines.Add(new InlineUIContainer { Child = grid });

        ChangelogRichTextBlock.Blocks.Add(paragraph);
      }
    }

    private static async Task<JsonArray> ReadFile(string url)
    {
      var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(url));
      var content = await FileIO.ReadTextAsync(file);

      var versions = JsonArray.Parse(content);
      return versions;
    }
  }
}