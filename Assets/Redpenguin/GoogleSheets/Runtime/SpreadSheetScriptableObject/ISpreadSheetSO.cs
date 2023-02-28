using System;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets
{
  public interface ISpreadSheetSO
  {
    public string SerializationGroupTag { get; set; }
    public bool IsLoad { get; set; }
    public string JsonSerialized { get; }
    public ISheetDataContainer SheetDataContainer { get; }
    public Type SheetDataType { get; }
    public void SetListCount(int count);
  }
}