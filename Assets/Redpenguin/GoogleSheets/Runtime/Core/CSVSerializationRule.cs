using System;
using System.IO;
using Newtonsoft.Json;
using Redpenguin.GoogleSheets.Scripts.Runtime.Core;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Runtime.Core
{
  [Serializable]
  public class CSVSerializationRule : SerializationRule
  {
    public CSVSerializationRule()
    {
      extension = "csv";
      packSeparately = true;
    }
    public override void Serialization(object objectToWrite)
    {
      foreach (var sheetContainerSerializeConfig in googleSheetSerializeConfig.sheetContainerSerializeConfigs)
      {
        var obj = ((ISpreadSheetSO) objectToWrite);
        if (sheetContainerSerializeConfig.containerType == obj.SheetDataTypeName)
        {
          var sheetContainer = obj.SheetDataContainer;
          var fname = $"{sheetContainerSerializeConfig.fileName}.{extension}";
          var fpath = Path.Combine(Application.dataPath, sheetContainerSerializeConfig.savePath);
          if (!Directory.Exists(fpath))
          {
            Directory.CreateDirectory(fpath);
          }
          var path = Path.Combine(fpath, fname);
          using var file = File.CreateText(path);
          var serializer = new JsonSerializer();
          serializer.Serialize(file, sheetContainer);
        }
        
        
      }
    }

    public override T Deserialization<T>(string text)
    {
      return JsonConvert.DeserializeObject<T>(text,
        new JsonSerializerSettings() {Converters = {new SpreadSheetsConverter()}});
    }
  }
}