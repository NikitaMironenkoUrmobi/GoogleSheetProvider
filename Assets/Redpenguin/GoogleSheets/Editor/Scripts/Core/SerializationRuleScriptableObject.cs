using Redpenguin.GoogleSheets.Runtime.Core;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Core
{
  public abstract class SerializationRuleSoWrapper : ScriptableObject, ISerializationRule
  {
    public abstract bool PackSeparately { get; }
    public abstract string FileName { get; }
    public abstract GoogleSheetSerializeConfig GoogleSheetSerializeConfig { get; }
    public abstract void Serialization(object objectToWrite);
  }
  public abstract class SerializationRuleScriptableObject<T> : SerializationRuleSoWrapper where T : SerializationRule
  {
    public T serializationRule;

    public override bool PackSeparately => serializationRule.packSeparately;

    public override GoogleSheetSerializeConfig GoogleSheetSerializeConfig => serializationRule.googleSheetSerializeConfig;

    public override string FileName => serializationRule.fileName;

    public override void Serialization(object objectToWrite)
    {
      serializationRule.Serialization(objectToWrite);
    }
  }

  public interface ISerializationRule
  {
    public bool PackSeparately { get; }

    public string FileName { get; }
    public GoogleSheetSerializeConfig GoogleSheetSerializeConfig { get; }
    public void Serialization(object objectToWrite);
  }
}