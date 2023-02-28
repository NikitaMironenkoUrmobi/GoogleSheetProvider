using System.Linq;
using Redpenguin.GoogleSheets.Settings;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Utils
{
  public static class GoogleSheetsProviderAssetMenu
  {
    
    private static void SelectSettingsAsset()
    {
      var providerSettingsList = AssetDatabaseHelper.FindAssetsByType<GoogleSheetProviderSettings>();
      if (providerSettingsList.Count > 1)
      {
        Debug.LogError($"Find {providerSettingsList.Count} GoogleSheetProviderSettings. Remove all except 1.");
      }

      if (providerSettingsList.Count == 0)
      {
        Debug.LogError($"Cant find GoogleSheetProviderSettings. Create via CreateAssetMenu -> Create -> GoogleSheets -> Settings.");
        return;
      }
      Selection.activeObject = providerSettingsList.First();
    }
    
    
  }
}