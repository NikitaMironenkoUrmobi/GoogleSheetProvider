using System;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Models
{
  [Serializable]
  public class ProfileModel
  {
    public string profileName = string.Empty;
    public Color color;
    public string tableID = string.Empty;
    public TextAsset credential;
    public string savePath = string.Empty;
    public string fileName = string.Empty;
    public string serializationRuleType = string.Empty;
    public bool selected;
  }
}