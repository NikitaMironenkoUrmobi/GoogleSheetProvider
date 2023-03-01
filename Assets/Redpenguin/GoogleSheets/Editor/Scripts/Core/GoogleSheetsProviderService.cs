using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Redpenguin.GoogleSheets.Attributes;
using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Editor.Factories;
using Redpenguin.GoogleSheets.Editor.Models;
using Redpenguin.GoogleSheets.Editor.Utils;
using Redpenguin.GoogleSheets.Settings;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Core
{
  public class GoogleSheetsProviderService : IDisposable
  {
    public List<Type> SpreadSheetDataTypes { get; private set; } = new();

    private readonly SpreadSheetCodeFactory _codeFactory = new();
    private readonly SpreadSheetScriptableObjectFactory _scriptObjFactory = new();

    private DataImporter _dataImporter;
    private GoogleSheetsReader _googleSheetsReader;
    public List<SpreadSheetSoWrapper> SpreadSheetSoList { get; }
    public ProfilesContainer ProfilesContainer { get; }

    public GoogleSheetsProviderService()
    {
      ProfilesContainer = AssetDatabaseHelper.FindAssetsByType<ProfilesContainer>()[0];

      var currentProfile = ProfilesContainer.CurrentProfile;
      _googleSheetsReader = new GoogleSheetsReader(currentProfile.tableID, currentProfile.credential.text);
      _dataImporter = new DataImporter(_googleSheetsReader);
      SpreadSheetSoList = AssetDatabaseHelper.FindAssetsByType<SpreadSheetSoWrapper>();
    }


    public string CantFindClassWithAttribute()
    {
      return _codeFactory.CantFindClass();
    }

    public void OnProfileChange()
    {
      var currentProfile = ProfilesContainer.CurrentProfile;
      _googleSheetsReader = new GoogleSheetsReader(currentProfile.tableID, currentProfile.credential.text);
      _dataImporter = new DataImporter(_googleSheetsReader);
      FindAllContainers();
    }

    public void FindAllContainers()
    {
      SpreadSheetDataTypes.Clear();
      var tableSheets = GetCurrentProfileTableSheetsNames();
      var types = GetContainersType(tableSheets);
      types.ForEach(x => SpreadSheetDataTypes.Add(x));
      // AssetDatabaseHelper
      //   .FindAssetsByType<SpreadSheetSoWrapper>().Where(x => types.Contains(x.SheetDataType))
      //   .ToList()
      //   .ForEach(x => SpreadSheetContainers.Add(x));
    }

    private List<string> GetCurrentProfileTableSheetsNames()
    {
      var tableSheets = ProfilesContainer.CurrentProfile.metaData.tableSheetsNames;
      if (tableSheets.Count == 0)
      {
        _googleSheetsReader.GetTableModel(ProfilesContainer.CurrentProfile.tableID).SheetNames
          .ForEach(x => tableSheets.Add(x));
        EditorUtility.SetDirty(ProfilesContainer);
      }

      return tableSheets;
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
      //_googleSheetsReader = new GoogleSheetsReader(currentProfile.tableID, currentProfile.credential.text);
      //_dataImporter = new DataImporter(_googleSheetsReader);
      
      var tableSheets = GetCurrentProfileTableSheetsNames();
      var types = GetContainersType(tableSheets);
      var list = CreateSheetDataContainers(types);
      if (list.Count == 0)
      {
        Debug.LogError(
          $"Can't find class with SpreadSheet attribute that has SheetName which contains in {currentProfile.profileName} profile table");
        return;
      }

      //_dataImporter.LoadAndFillDataContainers(SpreadSheetContainers.Select(x => x.SheetDataContainer).ToList());
      _dataImporter.LoadAndFillDataContainers(list);
      Serialize(list);
    }

    public void SerializeContainer(Type containerDataType)
    {
      var list = CreateSheetDataContainers(new List<Type>{containerDataType});
      _dataImporter.LoadAndFillDataContainers(list);
      Serialize(list);
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
      return _codeFactory.GetClassWithSpreadSheetAttribute().Count != SpreadSheetDataTypes.Count;
    }

    public void Serialization()
    {
      Serialize(SpreadSheetDataTypes.Select(x => (x as ISpreadSheetSO)?.SheetDataContainer).ToList());
    }

    private void Serialize(List<ISheetDataContainer> list)
    {
      var serialization = new Serialization(ProfilesContainer.CurrentProfile, list);
      serialization.SerializationByRule();
    }

    public void Dispose()
    {
      _googleSheetsReader?.Dispose();
    }

    public List<ISheetDataContainer> CreateSheetDataContainers(List<Type> types)
    {
      var list = new List<ISheetDataContainer>();
      types.ForEach(x =>
      {
        var constructedType = typeof(SpreadSheetDataContainer<>).MakeGenericType(x);
        var dataContainer = Activator.CreateInstance(constructedType) as ISheetDataContainer;
        list.Add(dataContainer);
      });

      return list;
    }

    public List<Type> GetContainersType(List<string> tableSheetsNames)
    {
      var listOfTypes = new List<Type>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        assembly.GetTypes().Where(type =>
            type.GetCustomAttribute(typeof(SpreadSheet)) is SpreadSheet spreadSheet
            && tableSheetsNames.Contains(spreadSheet.SheetName))
          .ToList().ForEach(x => { listOfTypes.Add(x); });
      }

      return listOfTypes;
    }
  }
}