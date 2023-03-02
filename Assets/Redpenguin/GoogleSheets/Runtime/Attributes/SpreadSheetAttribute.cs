using System;

namespace Redpenguin.GoogleSheets.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SpreadSheetAttribute : Attribute
  {
    public string SheetName;
    public string From;
    public string To;
    public string ProfileName;

    public string Range => $"{SheetName}!{From}:{To}";
    public SpreadSheetAttribute(string sheetName, string from = "A1", string to = "Z1000", string profileName = "")
    {
      ProfileName = profileName;
      SheetName = sheetName;
      From = from;
      To = to;
    }
  }
}