using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Settings.SerializationRules
{
  public class JsonSerializationRule : SerializationRule
  {
    public override void Serialization(string filePath, string fileName, object objectToWrite)
    {
      var fname = $"{fileName}.json";
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
  public class CSVSerializationRule : SerializationRule
  {
    public override void Serialization(string filePath, string fileName, object objectToWrite)
    {
      var fname = $"{fileName}CSV.json";
      
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