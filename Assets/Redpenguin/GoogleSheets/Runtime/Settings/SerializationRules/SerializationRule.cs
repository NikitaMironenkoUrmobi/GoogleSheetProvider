using System;

namespace Redpenguin.GoogleSheets.Settings.SerializationRules
{
  [Serializable]
  public abstract class SerializationRule
  {
    public bool packSeparately;
    public string filePath;
    public string fileName;
    public string extension;
    public GoogleSheetSerializeConfig googleSheetSerializeConfig;
    public abstract void Serialization(object objectToWrite);
    public abstract T Deserialization<T>(string text);
  }
}