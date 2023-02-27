using System;

namespace Redpenguin.GoogleSheets.Settings
{
  [Serializable]
  public class SheetContainerGroup
  {
    public string containerType;
    public string group = "Default";
    public bool isLoad = true;
  }
}