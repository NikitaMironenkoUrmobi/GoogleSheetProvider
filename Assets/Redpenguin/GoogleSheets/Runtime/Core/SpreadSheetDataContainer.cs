using System;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Attributes;

namespace Redpenguin.GoogleSheets.Core
{
  public interface ISheetDataProvider<T> : ISheetDataContainer where T : ISheetData
  {
    public List<T> Data { get; set; }
  }

  public interface ISheetDataContainer
  {
  }

  [Serializable]
  public class SpreadSheetDataContainer<T> : ISheetDataProvider<T> where T : ISheetData
  {
    public List<T> Data { get; set; }
    public string Type => GetType().ToString();

    public SpreadSheetDataContainer(List<T> data)
    {
      Data = data;
    }
  }
}