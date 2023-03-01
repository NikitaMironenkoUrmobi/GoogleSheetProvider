using System;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets
{
  public interface ISpreadSheetSO
  {
    public ISheetDataContainer SheetDataContainer { get; }
  }
}