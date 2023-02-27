using System;

namespace Redpenguin.GoogleSheets.Settings
{
  [Serializable]
  public class SheetContainerSerializeConfig 
  {
    public string containerType;
    public string savePath;
    public string loadPath;
    public string fileName;
  }
}