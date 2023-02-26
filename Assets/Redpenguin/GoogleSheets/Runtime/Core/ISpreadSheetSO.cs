using System.Collections.Generic;

namespace Redpenguin.GoogleSheets.Scripts.Runtime.Core
{
  public interface ISpreadSheetSO
  {
    public string SerializationGroupTag { get; set; }
    public string JsonSerialized { get; }
    public ISheetDataContainer SheetDataContainer { get; }
    public string SheetDataTypeName { get; }
    public void SetListCount(int count);
  }
}