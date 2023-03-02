using System;
using Redpenguin.GoogleSheets.Core;
using UnityEngine;

namespace Redpenguin.GoogleSheets
{
  public abstract class SpreadSheetSoWrapper: ScriptableObject, ISpreadSheetSoWrapper
  {
    public abstract ISheetDataContainer SheetDataContainer { get; }
    public abstract Type SheetDataType { get; }
    public abstract void SetListCount(int count);
  }

  public interface ISpreadSheetSoWrapper : ISheetDataContainer
  {
    public ISheetDataContainer SheetDataContainer { get; }
  }
}