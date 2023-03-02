using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Core
{
  public interface ISheetDataProvider<T> : ISheetDataContainer where T : ISheetData
  {
    public List<T> Data { get; set; }
  }

  public interface ISheetDataContainer
  {
    public Type SheetDataType { get; }
    void SetListCount(int count);
  }

  [Serializable]
  public class SpreadSheetDataContainer<T> : ISheetDataProvider<T> where T : ISheetData, new()
  {
    public List<T> data = new();

    [JsonIgnore] public List<T> Data
    {
      get => data;
      set => data = value;
    }
    [JsonIgnore] public Type SheetDataType => typeof(T);
    public Type ContainerType => GetType();
    public SpreadSheetDataContainer(List<T> data)
    {
      this.data = data;
    }

    public SpreadSheetDataContainer()
    {
    }
    
    public void SetListCount(int count)
    {
      var result = count - Data.Count;
      for (var i = 0; i < result; i++)
      {
        Data.Add(new T());
      }
    }

    
  }
}