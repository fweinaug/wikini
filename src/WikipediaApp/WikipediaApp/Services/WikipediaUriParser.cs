using System;
using System.Net;

namespace WikipediaApp
{
  public static class WikipediaUriParser
  {
    public static bool TryParseArticleUri(Uri uri, out string title, out string language, out string anchor)
    {
      title = null;
      language = null;
      anchor = null;

      if (!IsWikipediaUri(uri))
        return false;

      if (!uri.AbsolutePath.StartsWith("/wiki/"))
        return false;

      title = uri.AbsolutePath.Substring(6);

      if (title.StartsWith("File:"))
        return false;
      
      language = uri.Host.Substring(0, uri.Host.IndexOf('.'));

      if (uri.Fragment.StartsWith("#"))
        anchor = uri.Fragment.Substring(1);

      return true;
    }

    public static bool TryParseImageUri(Uri uri, out string filename)
    {
      if (uri.Scheme == "about" && uri.AbsolutePath == "blank")
      {
        var fragment = Uri.UnescapeDataString(uri.Fragment).Trim('#');
        if (fragment.StartsWith("/media/"))
        {
          var index = fragment.IndexOf(':');
          if (index > 0)
          {
            filename = fragment.Substring(index + 1);
            return true;
          }
        }
      }

      if (uri.Host.EndsWith(".wikipedia.org") && uri.AbsolutePath.StartsWith("/wiki/"))
      {
        var index = uri.AbsolutePath.IndexOf(':', 6);
        if (index > 0)
        {
          filename = WebUtility.UrlDecode(uri.AbsolutePath.Substring(index + 1));
          return true;
        }
      }

      filename = null;
      return false;
    }

    public static bool IsWikipediaUri(Uri uri)
    {
      return uri != null && uri.Host.EndsWith(".wikipedia.org");
    }
  }
}