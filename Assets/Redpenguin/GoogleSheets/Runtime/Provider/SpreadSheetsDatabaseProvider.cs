using Newtonsoft.Json;
using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Settings.SerializationRules;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Provider
{
  public class SpreadSheetsDatabaseProvider
  {
    public SpreadSheetsDatabase Database { get; private set; }

    public virtual void Load(string fileNameInput)
    {
      var serialization = new JsonSerializationRule();
      var fileName = fileNameInput;
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