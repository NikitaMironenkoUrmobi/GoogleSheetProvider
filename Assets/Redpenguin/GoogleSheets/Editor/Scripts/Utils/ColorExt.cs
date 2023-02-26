using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Core
{
  public static class ColorExt
  {
    public static Color MakeDarker(this Color color, int percent)
    {
      if (percent == 0) return color;
      var coeff = percent / 100f;
      return new Color(color.r - (color.r * coeff), color.g - (color.g * coeff), color.b - (color.b * coeff));
    }
  }
}