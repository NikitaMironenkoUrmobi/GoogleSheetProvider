using System;
using System.IO;
using Newtonsoft.Json;
using Redpenguin.GoogleSheets.Scripts.Runtime.Core;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Runtime.Core
{
  [Serializable]
  public class DefaultSerializationRule : SerializationRule
  {
    public DefaultSerializationRule()
    {
      filePath = "Resources";
      fileName = "SpreadSheetDatabase";
      extension = "json";
      packSeparately = false;
    }
    public override void Serialization(object objectToWrite)
    {
      var fname = $"{fileName}.{extension}";
      var fpath = Path.Combine(Application.dataPath, filePath);
      if (!Directory.Exists(fpath))
      {
        Directory.CreateDirectory(fpath);
      }

      var path = Path.Combine(fpath, fname);
      using var file = File.CreateText(path);
      var serializer = new JsonSerializer();
      serializer.Serialize(file, objectToWrite);
    }

    public override T Deserialization<T>(string text)
    {
      return JsonConvert.DeserializeObject<T>(text,
        new JsonSerializerSettings() {Converters = {new SpreadSheetsConverter()}});
    }
  }
}