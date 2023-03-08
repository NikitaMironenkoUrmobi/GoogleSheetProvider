using System;
using System.Collections.Generic;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData
{
  [Serializable]
  public class ProfileMetaData
  {
    public bool useSoContainers = false;
    public bool loadFromRemote = true;

    [SerializeField] private List<SheetContainerMetaData> sheetContainerMetaData = new();
    public List<string> tableSheetsNames = new();

    public SheetContainerMetaData GetMeta(string containerDataType)
    {
      var data = sheetContainerMetaData.Find(x => x.containerType == containerDataType);
      if (data == null)
      {
        data = new SheetContainerMetaData {containerType = containerDataType};
        sheetContainerMetaData.Add(data);
      }

      return data;
    }
  }
}