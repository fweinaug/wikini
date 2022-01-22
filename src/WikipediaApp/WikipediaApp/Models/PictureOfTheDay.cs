using System;
using System.Collections.Generic;

namespace WikipediaApp
{
  public class PictureOfTheDay
  {
    public Uri ImageUri { get; set; }
    public Uri ThumbnailUri { get; set; }
    public IReadOnlyDictionary<string, string> Descriptions { get; set; }

    public bool HasDescription
    {
      get { return Descriptions != null && Descriptions.Count > 0; }
    }

    public Description GetDescription(string language)
    {
      if (!HasDescription)
        return null;

      if (language != "en" && Descriptions.TryGetValue(language, out var description))
        return new Description(language, description);

      if (Descriptions.TryGetValue("en", out var descriptionEN))
        return new Description("en", descriptionEN);

      return null;
    }
  }

  public class Description
  {
    public string Language { get; }
    public string Text { get; }

    public Description(string language, string text)
    {
      Language = language;
      Text = text;
    }
  }
}