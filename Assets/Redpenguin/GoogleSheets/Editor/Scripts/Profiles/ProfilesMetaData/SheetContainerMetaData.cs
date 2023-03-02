using System;
using UnityEngine.Serialization;

namespace Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData
{
  [Serializable]
  public class SheetContainerMetaData
  {
    public string containerType;
    public bool isLoad = true;
  }
}