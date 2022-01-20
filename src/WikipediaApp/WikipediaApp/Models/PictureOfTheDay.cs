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
  }
}