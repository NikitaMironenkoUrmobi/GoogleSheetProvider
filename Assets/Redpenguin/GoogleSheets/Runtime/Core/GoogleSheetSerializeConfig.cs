using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Redpenguin.GoogleSheets.Runtime.Core
{
  [Serializable]
  public class GoogleSheetSerializeConfig
  {
    public List<SheetContainerSerializeConfig> sheetContainerSerializeConfigs = new();
    public SheetContainerSerializeConfig Get(string containerType)
    {
      var data = sheetContainerSerializeConfigs.Find(x => x.containerType == containerType);
      if (data == null)
      {
        data = new SheetContainerSerializeConfig {containerType = containerType};
        sheetContainerSerializeConfigs.Add(data);
      }
      return data;
    }
  }
}