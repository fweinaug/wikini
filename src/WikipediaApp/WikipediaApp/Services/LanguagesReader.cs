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

      for (var i = 0; i < array.Count; i++)
      {
        var value = array[i];
        var obj = value.GetObject();

        var visible = obj.GetNamedBoolean("Visible");
        if (!visible)
          continue;

        var name = obj.GetNamedString("Name");
        var code = obj.GetNamedString("Code");
        var url = obj.GetNamedString("Url");

        var language = new Language
        {
          Name = name,
          Code = code,
          Uri = new Uri(url),
          Index = i
        };

        list.Add(language);
      }

      return list;
    }
  }
}