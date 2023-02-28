using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Editor.Factories;
using Redpenguin.GoogleSheets.Editor.Models;
using Redpenguin.GoogleSheets.Editor.Utils;
using Redpenguin.GoogleSheets.Settings;
using Redpenguin.GoogleSheets.Settings.SerializationRules;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Core
{
  public class GoogleSheetsProviderService : IDisposable
  {
    public List<ScriptableObject> SpreadSheetContainers { get; private set; } = new();
    public GoogleSheetProviderSettings Settings { get; set; }


    private readonly SpreadSheetCodeFactory _codeFactory = new();
    private readonly SpreadSheetScriptableObjectFactory _scriptObjFactory = new();

    private DataImporter _dataImporter;
    private GoogleSheetsReader _googleSheetsReader;
    public TableModel CurrentTableModel { get; private set; }
    public ProfilesContainer ProfilesContainer { get; }

    public GoogleSheetsProviderService()
    {
      ProfilesContainer = AssetDatabaseHelper.FindAssetsByType<ProfilesContainer>()[0];
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
      var currentProfile = ProfilesContainer.CurrentProfile;
      _googleSheetsReader = new GoogleSheetsReader(currentProfile.tableID, currentProfile.credential.text);
      CurrentTableModel = _googleSheetsReader.GetTableModel();

      _dataImporter = new DataImporter(_googleSheetsReader);
      _dataImporter.LoadAndLinkSheetsDataToSo(SpreadSheetContainers);
      //SaveToFile();
    }


    public void CreateAdditionalScripts(Action<bool> isCreatedCallback)
    {
      var currentProfile = ProfilesContainer.CurrentProfile;
      _scriptObjFactory.DeleteAllAssets(currentProfile.profileName);
      _codeFactory.DeleteAllScripts(currentProfile.profileName);
      var isCreated = _codeFactory.CreateAdditionalScripts(currentProfile.profileName);
      isCreatedCallback.Invoke(isCreated);
    }

    public void CreateScriptableObjects()
    {
      var currentProfile = ProfilesContainer.CurrentProfile;
      _scriptObjFactory.CreateScriptableObjects(_codeFactory.GetGeneratedScriptsTypes(), currentProfile.profileName);
      FindAllContainers();
    }

    public bool CanCreateContainers()
    {
      return _codeFactory.GetClassWithSpreadSheetAttribute().Count != SpreadSheetContainers.Count;
    }

    // public void SaveAllGroups()
    // {
    //   
    //   foreach (var settingsSerializationGroup in Settings.SerializationGroups)
    //   {
    //     if (settingsSerializationGroup.serializationRule == null)
    //     {
    //       Debug.LogError($"SerializationRule for {Settings.currentGroup.tag} Group doesn't exist.");
    //       continue;
    //     }
    //     
    //     var rule = settingsSerializationGroup.serializationRule;
    //     if (rule.PackSeparately)
    //     {
    //       foreach (var spreadSheetContainer in SpreadSheetContainers)
    //       {
    //         var sr = (spreadSheetContainer as ISpreadSheetSO);
    //         if(sr.SerializationGroupTag == settingsSerializationGroup.tag)
    //           rule.Serialization(sr);
    //       }
    //     }
    //     else
    //     {
    //       var container = new SpreadSheetsDatabase();
    //       foreach (var spreadSheetContainer in SpreadSheetContainers)
    //       {
    //         var sr = (spreadSheetContainer as ISpreadSheetSO);
    //         if(sr.SerializationGroupTag == settingsSerializationGroup.tag)
    //          container.AddContainer(sr.SheetDataContainer);
    //       }
    //       rule.Serialization(container);
    //       Debug.Log($"{rule.FileName} save to file!");
    //     }
    //   }
    //   
    //   AssetDatabase.Refresh();
    // }
    public void SaveToFile()
    {
      var currentProfile = ProfilesContainer.CurrentProfile;
      var container = new SpreadSheetsDatabase();
      foreach (var spreadSheetContainer in SpreadSheetContainers)
      {
        var sr = (spreadSheetContainer as ISpreadSheetSO);
        container.AddContainer(sr.SheetDataContainer);
      }

      if (currentProfile.serializationRuleType == string.Empty)
      {
        Debug.LogError($"SerializationRule for {currentProfile.profileName} doesn't exist.");
        return;
      }

      var t = new JsonSerializationRule();
      var tr = t.GetType().ToString();
      var tr2 = t.GetType().FullName;
      
      var type = Type.GetType(currentProfile.serializationRuleType);
      var serializator =
        Activator.CreateInstance(type) as SerializationRule;
      serializator.Serialization(currentProfile.savePath, currentProfile.fileName, container);
      AssetDatabase.Refresh();
      //
      // if (Settings.currentGroup.serializationRule == null)
      // {
      //   Debug.LogError($"SerializationRule for {Settings.currentGroup.tag} Group doesn't exist.");
      //   return;
      // }
      // var rule = Settings.currentGroup.serializationRule;
      // if (rule.PackSeparately)
      // {
      //   foreach (var spreadSheetContainer in SpreadSheetContainers)
      //   {
      //     var sr = (spreadSheetContainer as ISpreadSheetSO);
      //     if(sr.SerializationGroupTag == Settings.currentGroup.tag)
      //       rule.Serialization(sr);
      //   }
      // }
      // else
      // {
      //   rule.Serialization(container);
      //   Debug.Log($"{rule.FileName} save to file!".WithColor(ColorExt.CompletedColor));
      // }
      //
      //
      // AssetDatabase.Refresh();
    }

    public void Dispose()
    {
      _googleSheetsReader?.Dispose();
    }
  }
}