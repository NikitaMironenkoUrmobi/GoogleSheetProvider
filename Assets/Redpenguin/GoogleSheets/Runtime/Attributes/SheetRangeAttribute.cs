using System;

namespace Redpenguin.GoogleSheets.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SheetRangeAttribute : Attribute
  {
    public readonly string SpreadSheetRange;
    public Type DataType;

    public SheetRangeAttribute(string spreadSheetRange, Type dataType)
    {
      DataType = dataType;
      SpreadSheetRange = spreadSheetRange;
    }
  }
}