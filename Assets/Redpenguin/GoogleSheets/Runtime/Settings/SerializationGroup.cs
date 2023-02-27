using System;
using Redpenguin.GoogleSheets.Settings.SerializationRules;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Settings
{
  [Serializable]
  public class SerializationGroup
  {
    public string tag;
    public Color color;
    public SerializationRuleSoWrapper serializationRule;
  }
}