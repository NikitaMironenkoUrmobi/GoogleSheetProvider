using System;
using UnityEngine.Serialization;
using static System.String;

namespace Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData
{
  [Serializable]
  public class SheetContainerMetaData
  {
    public string containerType;
    public bool isLoad = true;
    public bool saveSeparately;
    public bool overrideName;
    public string savePath = Empty;
    public string fileName = Empty;
  }
}