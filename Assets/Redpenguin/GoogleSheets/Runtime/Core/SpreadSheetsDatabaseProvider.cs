using Redpenguin.GoogleSheets.Settings;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Core
{
  public class SpreadSheetsDatabaseProvider
  {
    private readonly SerializeSettingsContainer _serializeSettings;

    public SpreadSheetsDatabaseProvider()
    {
      _serializeSettings = Resources.Load<SerializeSettingsContainer>("SerializeSettingsContainer");
    }
    public virtual SpreadSheetsDatabase Load(string profileName)
    {
      var serialization = new SpreadSheetsSerializer(profileName, _serializeSettings);
      return serialization.DeserializeByRule();
    }
  }
}