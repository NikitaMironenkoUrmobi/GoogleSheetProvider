using System;
using System.Collections.Generic;
using System.Linq;
using Redpenguin.GoogleSheets.Attributes;

namespace Redpenguin.GoogleSheets.Core
{
  [Serializable]
  public class SpreadSheetsDatabase
  {
    public List<ISheetDataContainer> Containers { get; set; } = new();

    public T GetContainer<T>() where T : ISheetDataContainer
    {
      foreach (var container in Containers.OfType<T>())
      {
        return container;
      }

      throw new ArgumentNullException();
    }

    public List<T> GetSpreadSheetData<T>() where T : ISheetData, new()
    {
      foreach (var container in Containers.OfType<SpreadSheetDataContainer<T>>())
      {
        return container.Data;
      }

      throw new Exception($"SpreadSheetsDatabase doesn't have container with data of {typeof(T)} type");
    }
    public void SetData<T>(List<T> list) where T : ISheetData, new()
    {
      foreach (var container in Containers.OfType<SpreadSheetDataContainer<T>>())
      {
        container.Data = list;
      }
    }

    public void AddContainer(ISheetDataContainer container)
    {
      if (!Containers.Contains(container))
        Containers.Add(container);
    }
  }
}