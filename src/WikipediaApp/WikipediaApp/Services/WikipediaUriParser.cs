using System;

namespace WikipediaApp
{
  public static class WikipediaUriParser
  {
    public static bool Parse(Uri uri, out string title, out string language, out string anchor)
    {
      title = null;
      language = null;
      anchor = null;

      if (!IsWikipediaUri(uri))
        return false;

      if (!uri.AbsolutePath.StartsWith("/wiki/"))
        return false;

      title = uri.AbsolutePath.Substring(6);
      language = uri.Host.Substring(0, uri.Host.IndexOf('.'));

      if (uri.Fragment.StartsWith("#"))
        anchor = uri.Fragment.Substring(1);

      return true;
    }

    public static bool IsWikipediaUri(Uri uri)
    {
      return uri != null && uri.Host.EndsWith(".wikipedia.org");
    }
  }
}