using Redpenguin.GoogleSheets.Core;
using UnityEngine;

namespace Redpenguin.GoogleSheets
{
  public abstract class SpreadSheetSoWrapper: ScriptableObject, ISpreadSheetSO
  {
    public abstract string SerializationGroupTag { get; set; }
    public abstract bool IsLoad { get; set; }
    public abstract string JsonSerialized { get; }
    public abstract ISheetDataContainer SheetDataContainer { get; }
    public abstract string SheetDataTypeName { get; }
    public abstract void SetListCount(int count);
  }
}