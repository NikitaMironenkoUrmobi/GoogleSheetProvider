using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Settings
{
  [CreateAssetMenu(menuName = "Create SerializeSettingsContainer", fileName = "SerializeSettingsContainer", order = 0)]
  public class SerializeSettingsContainer : ScriptableObject
  {
    [SerializeField] private List<SerializeSetting> serializeSettings = new();
    [SerializeField] private List<SerializationRuleSetting> serializationRuleSetting = new();

    public SerializeSetting GetSerializeSetting(string profile, string containerType)
    {
      var data = serializeSettings.Find(x => x.profile == profile && x.containerType == containerType);
      if (data == null)
      {
        data = new SerializeSetting {profile = profile, containerType = containerType};
        serializeSettings.Add(data);
      }

      return data;
    }

    public List<SerializeSetting> GetSerializeSetting(string profile)
    {
      var data = serializeSettings.Where(x => x.profile == profile).ToList();
      if (data.Count == 0)
      {
        Debug.LogError($"Can't find serialization settings for {profile} profile.");
      }

      return data;
    }

    public SerializationRuleSetting GetSerializeRuleSetting(string profile)
    {
      var data = serializationRuleSetting.Find(x => x.profile == profile);
      if (data == null)
      {
        data = new SerializationRuleSetting {profile = profile, serializationRuleType = ""};
        serializationRuleSetting.Add(data);
        Debug.LogWarning($"Can't find serialization rule settings for {profile} profile.");
      }

      return data;
    }

    public void SetSerializeRuleSetting(string profile, string serializationRuleType)
    {
      var data = serializationRuleSetting.Find(x => x.profile == profile);
      if (data == null)
      {
        serializationRuleSetting.Add(new SerializationRuleSetting
          {profile = profile, serializationRuleType = serializationRuleType});
      }
      else
      {
        data.serializationRuleType = serializationRuleType;
      }
    }

    public void Clear(string profile)
    {
      foreach (var serializeSetting in serializeSettings.Where(x => x.profile == profile).ToList())
      {
        serializeSettings.Remove(serializeSetting);
      }
    }
  }
}