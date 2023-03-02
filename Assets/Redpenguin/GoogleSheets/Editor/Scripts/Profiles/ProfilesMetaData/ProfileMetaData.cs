using System;
using System.Collections.Generic;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData
{
  [Serializable]
  public class ProfileMetaData
  {
    public bool useSoContainers = true;
    public bool loadFromRemote = true;

    [SerializeField] private List<SheetContainerMetaData> sheetContainerMetaData = new();
    public List<string> tableSheetsNames = new();

    public SheetContainerMetaData GetMeta(string containerType)
    {
      var data = sheetContainerMetaData.Find(x => x.containerType == containerType);
      if (data == null)
      {
        data = new SheetContainerMetaData {containerType = containerType};
        sheetContainerMetaData.Add(data);
      }

      return data;
    }
  }
}