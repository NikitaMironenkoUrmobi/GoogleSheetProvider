using System;
using Redpenguin.GoogleSheets.Scripts.Editor.Core;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor
{
  [Serializable]
  public class SerializationGroup
  {
    public string tag;
    public Color color;
    public SerializationRuleSoWrapper serializationRule;
  }
}