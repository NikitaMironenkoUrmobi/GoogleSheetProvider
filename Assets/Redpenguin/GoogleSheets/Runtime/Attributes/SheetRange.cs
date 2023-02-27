using System;

namespace Redpenguin.GoogleSheets.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SheetRange : Attribute
  {
    public readonly string SpreadSheetRange;

    public SheetRange(string spreadSheetRange)
    {
      SpreadSheetRange = spreadSheetRange;
    }
  }
}