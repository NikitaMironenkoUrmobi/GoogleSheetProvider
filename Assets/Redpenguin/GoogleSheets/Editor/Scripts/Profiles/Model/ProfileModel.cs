using System;
using Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Profiles.Model
{
    [Serializable]
    public class ProfileModel
    {
        public string profileName = string.Empty;
        public Color color;
        public string tableID = string.Empty;
        public bool selected;
        public ProfileMetaData metaData = new();
        public string Guid;

        public string CredentialPath
        {
            set => EditorPrefs.SetString($"GoogleSheetCredentialPath_{Guid}", value);
            get => EditorPrefs.GetString($"GoogleSheetCredentialPath_{Guid}", "");
        }

        public bool IsValid => tableID != string.Empty && !string.IsNullOrEmpty(CredentialPath);
    }
}