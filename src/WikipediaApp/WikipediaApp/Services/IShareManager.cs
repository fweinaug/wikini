using System;

namespace WikipediaApp
{
  public interface IShareManager
  {
    void CopyToClipboard(Uri uri);
    void ShareArticle(string title, Uri uri);
  }
}