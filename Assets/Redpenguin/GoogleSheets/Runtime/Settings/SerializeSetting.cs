using System;
using System.Collections.Generic;
using static System.String;

namespace Redpenguin.GoogleSheets.Settings
{
  [Serializable]
  public class SerializeSetting
  {
    public string containerType;
    public bool saveSeparately;
    public bool overrideName;
    public string savePath = Empty;
    public string fileName = Empty;
  }
  [Serializable]
  public class SerializationRuleSetting
  {
    public string profile;
    public string serializationRuleType;
    public string savePath = Empty;
    public string fileName = Empty;
    public List<SerializeSetting> serializeSettings = new();
  }
}