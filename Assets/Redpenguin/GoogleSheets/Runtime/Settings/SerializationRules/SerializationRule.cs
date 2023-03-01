namespace Redpenguin.GoogleSheets.Settings.SerializationRules
{
  public abstract class SerializationRule
  {
    public abstract void Serialization(string filePath, string fileName, object objectToWrite);
    public abstract T Deserialization<T>(string text);
  }
}