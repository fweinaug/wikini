using System;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml;

namespace WikipediaApp
{
  public static class FadeInOutAnimation
  {
    private static readonly ImplicitAnimationSet showAnimations = new();
    private static readonly ImplicitAnimationSet hideAnimations = new();

    static FadeInOutAnimation()
    {
      showAnimations.Add(new OpacityAnimation { Duration = TimeSpan.FromMilliseconds(200), From = 0, To = 1 });
      hideAnimations.Add(new OpacityAnimation { Duration = TimeSpan.FromMilliseconds(150), From = 1, To = 0 });
    }

    public static void Enable(UIElement element)
    {
      Implicit.SetShowAnimations(element, showAnimations);
      Implicit.SetHideAnimations(element, hideAnimations);
    }

    public static void Disable(UIElement element)
    {
      Implicit.SetShowAnimations(element, null);
      Implicit.SetHideAnimations(element, null);
    }
  }
}