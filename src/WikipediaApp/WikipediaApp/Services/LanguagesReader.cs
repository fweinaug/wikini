using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace WikipediaApp
{
  public static class LanguagesReader
  {
    public static async Task<List<Language>> GetLanguages()
    {
      var uri = new Uri("ms-appx:///Data/languages.json");
      var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

      var content = await FileIO.ReadTextAsync(file);
      var array = JsonArray.Parse(content);

      var list = new List<Language>();

      foreach (var value in array)
      {
        var obj = value.GetObject();

        var name = obj.GetNamedString("Name");
        var code = obj.GetNamedString("Code");
        var url = obj.GetNamedString("Url");

        var language = new Language
        {
          Name = name,
          Code = code,
          Uri = new Uri(url),
        };

        list.Add(language);
      }

      return list;
    }
  }
}