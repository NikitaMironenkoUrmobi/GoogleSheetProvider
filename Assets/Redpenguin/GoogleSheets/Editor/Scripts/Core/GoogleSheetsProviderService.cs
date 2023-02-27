using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Editor.Factories;
using Redpenguin.GoogleSheets.Editor.Utils;
using Redpenguin.GoogleSheets.Settings;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Core
{
  public class GoogleSheetsProviderService : IDisposable
  {
    public List<ScriptableObject> SpreadSheetContainers { get; private set; } = new();
    public GoogleSheetProviderSettings Settings { get; set; }

    private readonly DataImporter _dataImporter;
    private readonly SpreadSheetCodeFactory _codeFactory = new();
    private readonly SpreadSheetScriptableObjectFactory _scriptObjFactory = new();
    
    private readonly GoogleSheetsReader _googleSheetsReader;

    public GoogleSheetsProviderService()
    {
      if (SetupSettings()) return;
      if (!IsSettingsSetup())
      {
        return;
      }
      _googleSheetsReader = new GoogleSheetsReader(Settings.googleSheetID, Settings.credential.text);
      _dataImporter = new DataImporter(_googleSheetsReader);
    }

    public bool IsSettingsSetup()
    {
      return IsGoogleSheetIdSetup() && IsCredentialSetup();
    }

    public bool IsGoogleSheetIdSetup()
    {
      return Settings.googleSheetID != "";
    }
    public bool IsCredentialSetup()
    {
      return Settings.credential != null && Settings.credential.text != "";
    }

    public string CantFindClassWithAttribute()
    {
      return _codeFactory.CantFindClass();
    }
    

    public void FindAllContainers()
    {
      SpreadSheetContainers.Clear();
      
      AssetDatabaseHelper
        .FindAssetsByType<SpreadSheetSoWrapper>()
        .ForEach(x => SpreadSheetContainers.Add(x));
    }

    private bool SetupSettings()
    {
      var providerSettingsList = AssetDatabaseHelper.FindAssetsByType<GoogleSheetProviderSettings>();
      switch (providerSettingsList.Count)
      {
        case > 1:
          Debug.LogError($"Find {providerSettingsList.Count} GoogleSheetProviderSettings. Remove all except 1.");
          break;
        case 0:
          Debug.LogError(
            $"Cant find GoogleSheetProviderSettings. Create via CreateAssetMenu -> Create -> GoogleSheets -> Settings.");
          return true;
      }

      Settings = providerSettingsList.First();
      return false;
    }
    
    public void Clear()
    {
      Caching.ClearCache();
      PlayerPrefs.DeleteAll();
      var di = new DirectoryInfo(GoogleSheetsVariables.SavePaths.DATABASE_SAVEPATH);
      if (!di.Exists) return;
      foreach (var file in di.EnumerateFiles())
      {
        Debug.Log($"{file.Name} delete");
        file.Delete();
      }
    }
    public void LoadSheetsData()
    {
      _dataImporter.LoadAndLinkSheetsDataToSo(SpreadSheetContainers);
      //SaveToFile();
    }

    
    public void CreateAdditionalScripts(Action<bool> isCreatedCallback)
    {
      _scriptObjFactory.DeleteAllAssets();
      _codeFactory.DeleteAllScripts();
      var isCreated =  _codeFactory.CreateAdditionalScripts();
      isCreatedCallback.Invoke(isCreated);
    }

    public void CreateScriptableObjects()
    {
      _scriptObjFactory.CreateScriptableObjects(_codeFactory.GetGeneratedScriptsTypes());
      FindAllContainers();
    }

    public bool CanCreateContainers()
    {
      return _codeFactory.GetClassWithSpreadSheetAttribute().Count != SpreadSheetContainers.Count;
    }

    public void SaveAllGroups()
    {
      
      foreach (var settingsSerializationGroup in Settings.SerializationGroups)
      {
        if (settingsSerializationGroup.serializationRule == null)
        {
          Debug.LogError($"SerializationRule for {Settings.currentGroup.tag} Group doesn't exist.");
          continue;
        }
        
        var rule = settingsSerializationGroup.serializationRule;
        if (rule.PackSeparately)
        {
          foreach (var spreadSheetContainer in SpreadSheetContainers)
          {
            var sr = (spreadSheetContainer as ISpreadSheetSO);
            if(sr.SerializationGroupTag == settingsSerializationGroup.tag)
              rule.Serialization(sr);
          }
        }
        else
        {
          var container = new SpreadSheetsDatabase();
          foreach (var spreadSheetContainer in SpreadSheetContainers)
          {
            var sr = (spreadSheetContainer as ISpreadSheetSO);
            if(sr.SerializationGroupTag == settingsSerializationGroup.tag)
             container.AddContainer(sr.SheetDataContainer);
          }
          rule.Serialization(container);
          Debug.Log($"{rule.FileName} save to file!");
        }
      }
      
      AssetDatabase.Refresh();
    }
    public void SaveToFile()
    {
      //((ISpreadSheetSave) _configDatabase).SaveToFile();
      var container = new SpreadSheetsDatabase();
      foreach (var spreadSheetContainer in SpreadSheetContainers)
      {
        var sr = (spreadSheetContainer as ISpreadSheetSO);
        if(sr.SerializationGroupTag == Settings.currentGroup.tag)
          container.AddContainer(sr.SheetDataContainer);
      }

      if (Settings.currentGroup.serializationRule == null)
      {
        Debug.LogError($"SerializationRule for {Settings.currentGroup.tag} Group doesn't exist.");
        return;
      }
      var rule = Settings.currentGroup.serializationRule;
      if (rule.PackSeparately)
      {
        foreach (var spreadSheetContainer in SpreadSheetContainers)
        {
          var sr = (spreadSheetContainer as ISpreadSheetSO);
          if(sr.SerializationGroupTag == Settings.currentGroup.tag)
            rule.Serialization(sr);
        }
      }
      else
      {
        rule.Serialization(container);
        Debug.Log($"{rule.FileName} save to file!".WithColor(ColorExt.CompletedColor));
      }
      
      
      AssetDatabase.Refresh();
    }

    public void Dispose()
    {
      
      _googleSheetsReader?.Dispose();
    }
  }
}