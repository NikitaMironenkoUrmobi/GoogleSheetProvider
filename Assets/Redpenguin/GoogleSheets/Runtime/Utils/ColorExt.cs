using UnityEngine;

namespace Redpenguin.GoogleSheets
{
  public static class ColorExt
  {
    public const string WaitingColor = "#595bde";
    public const string CompletedColor = "#a4e04f";
    public const string ErrorColor = "#d95041";

    public static Color MakeDarker(this Color color, int percent)
    {
      if (percent == 0) return color;
      var coeff = percent / 100f;
      return new Color(color.r - (color.r * coeff), color.g - (color.g * coeff), color.b - (color.b * coeff));
    }

    public static string WithColor(this string text, string color)
    {
      return $"<color={color}>{text}</color>";
    }
  }
}