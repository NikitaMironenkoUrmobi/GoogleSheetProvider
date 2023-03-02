using System;
using Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Profiles.Model
{
  [Serializable]
  public class ProfileModel
  {
    public string profileName = string.Empty;
    public Color color;
    public string tableID = string.Empty;
    public TextAsset credential;
    public bool selected;
    public ProfileMetaData metaData;
  }
}