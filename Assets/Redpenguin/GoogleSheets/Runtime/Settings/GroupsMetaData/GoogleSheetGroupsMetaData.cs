using System;
using System.Collections.Generic;

namespace Redpenguin.GoogleSheets.Settings
{
  [Serializable]
  public class GoogleSheetGroupsMetaData
  {
    public List<SheetContainerGroup> sheetContainerSerializeConfigs = new();
    public SheetContainerGroup Get(string containerType)
    {
      var data = sheetContainerSerializeConfigs.Find(x => x.containerType == containerType);
      if (data == null)
      {
        data = new SheetContainerGroup {containerType = containerType};
        sheetContainerSerializeConfigs.Add(data);
      }
      return data;
    }
  }
}