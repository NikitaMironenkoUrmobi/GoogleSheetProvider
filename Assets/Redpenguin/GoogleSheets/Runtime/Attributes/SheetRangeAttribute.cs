using System;

namespace Redpenguin.GoogleSheets.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SheetRangeAttribute : Attribute
  {
    public readonly string SpreadSheetRange;
    public Type DataType;
    public readonly string Profile;
    public SheetRangeAttribute(string spreadSheetRange, Type dataType, string profile)
    {
      DataType = dataType;
      SpreadSheetRange = spreadSheetRange;
      Profile = profile;
    }
  }
}