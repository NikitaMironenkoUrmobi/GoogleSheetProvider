using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Redpenguin.GoogleSheets.Attributes;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets
{
  public abstract class SpreadSheetScriptableObject<T> : SpreadSheetSoWrapper, ISheetDataProvider<T>
    where T : ISheetData, new()
  {
    public string serializationGroupTag = "Default";
    public override string JsonSerialized => JsonConvert.SerializeObject(SheetDataContainer);
    public override ISheetDataContainer SheetDataContainer => new SpreadSheetDataContainer<T>(data);

    public override Type SheetDataType => typeof(T);
    public bool isLoad;

    public override bool IsLoad
    {
      get => isLoad;
      set => isLoad = value;
    }

    public string Type => GetType().ToString();
    public List<T> data = new();

    public override string SerializationGroupTag
    {
      get => serializationGroupTag;
      set => serializationGroupTag = value;
    }

    [JsonIgnore]
    public List<T> Data
    {
      get => data;
      set => data = value;
    }

    public override void SetListCount(int count)
    {
      var result = count - data.Count;
      for (var i = 0; i < result; i++)
      {
        data.Add(new T());
      }
    }
  }
}