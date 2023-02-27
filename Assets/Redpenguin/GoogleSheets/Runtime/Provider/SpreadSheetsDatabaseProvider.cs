using Newtonsoft.Json;
using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Settings.SerializationRules;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Provider
{
  public class SpreadSheetsDatabaseProvider
  {
    public SpreadSheetsDatabase Database { get; private set; }

    public virtual void Load()
    {
      var serialization = Resources.Load<DefaultSerializationRuleScriptableObject>("SerializationRules/DefaultSerializationRule").serializationRule;
      var fileName = serialization.fileName;
      var fileTemplate = Resources.Load<TextAsset>($"{fileName}");
      if (fileTemplate != null)
      {
        
        Database = serialization.Deserialization<SpreadSheetsDatabase>(fileTemplate.text);
        Debug.Log($"<color=green>Load {fileName} from Resources/{fileName}</color>");
      }
      else
      {
        Database = new SpreadSheetsDatabase();
        Debug.LogError("File template dont find!");
      }

      AdditionalLoad();
    }

    protected virtual void AdditionalLoad()
    {
    }
  }
}