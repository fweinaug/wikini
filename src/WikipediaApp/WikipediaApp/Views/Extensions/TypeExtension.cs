using System;
using Windows.UI.Xaml.Markup;

namespace WikipediaApp
{
  [MarkupExtensionReturnType(ReturnType = typeof(Type))]
  public sealed class TypeExtension : MarkupExtension
  {
    public string TypeName { get; set; }

    protected override object ProvideValue()
    {
      return Type.GetType(TypeName);
    }
  }
}