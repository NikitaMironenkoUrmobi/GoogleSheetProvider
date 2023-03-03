using System.Collections.Generic;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Settings
{
  [CreateAssetMenu(menuName = "Create SerializeSettingsContainer", fileName = "SerializeSettingsContainer", order = 0)]
  public class SerializeSettingsContainer : ScriptableObject
  {
    [SerializeField] private List<SerializationRuleSetting> serializationRuleSetting = new();

    public SerializeSetting GetSerializeSetting(string profile, string containerType)
    {
      var rule = GetSerializeRuleSetting(profile);
      var data = rule.serializeSettings.Find(x => x.containerType == containerType);
      if (data == null)
      {
        data = new SerializeSetting {containerType = containerType};
        rule.serializeSettings.Add(data);
      }
      return data;
    }

    public List<SerializeSetting> GetSerializeSetting(string profile)
    {
      var data = serializationRuleSetting.Find(x => x.profile == profile);
      if (data == null)
      {
        Debug.LogError($"Can't find serialization settings for {profile} profile.");
        return new List<SerializeSetting>();
      }

      return data.serializeSettings;
    }

    public void RemoveSerializeRuleSetting(string profile)
    {
      var data = serializationRuleSetting.Find(x => x.profile == profile);
      if (data != null)
      {
        serializationRuleSetting.Remove(data);
      }
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
  }
}