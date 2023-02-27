using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Redpenguin.GoogleSheets.Settings
{
  [CreateAssetMenu(fileName = "GoogleSheetProviderSettings", menuName = "GoogleSheetProvider/Settings")]
  public class GoogleSheetProviderSettings : ScriptableObject
  {
    public string googleSheetID;
    public TextAsset credential;
    public List<SerializationGroup> serializationGroups;
    
    public SerializationGroup defaultGroup = new()
    {
      tag = "Default",
      color = new Color32(87, 165, 140, 255)
    };
     
    public GoogleSheetGroupsMetaData groupsMetaData;
    private void Awake()
    {
      currentGroup ??= defaultGroup;
    }
    
    public SerializationGroup currentGroup;
    public List<SerializationGroup> SerializationGroups => GetSerializationGroups();

    private List<SerializationGroup> GetSerializationGroups()
    {
      var list = new List<SerializationGroup>(serializationGroups);
      if (list.Count == 0)
      {
        list.Add(defaultGroup);
      }
      else
      {
        list.Insert(0,defaultGroup);
      }
        
      return list;
    }
  }
}