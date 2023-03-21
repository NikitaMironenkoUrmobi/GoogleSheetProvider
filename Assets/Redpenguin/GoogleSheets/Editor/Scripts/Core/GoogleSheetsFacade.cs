using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Redpenguin.GoogleSheets.Attributes;
using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Editor.Factories;
using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using Redpenguin.GoogleSheets.Settings;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Core
{
  public interface IGoogleSheetsFacade
  {
    void SerializeSheetData();
    void SearchForSpreadSheets();
    public List<Type> SpreadSheetDataTypes { get; }
    List<ISpreadSheetSoWrapper> SpreadSheetSoList { get; }
    void CantFindClassWithAttribute();
    void ClearAllData();
    bool CanCreateContainers();
    void CreateAdditionalScripts(Action<bool> action);
    void SerializeContainer(Type dataType);
    void CreateScriptableObjects();
    void SetupContainers(List<ISpreadSheetSoWrapper> list);
    void Dispose();
    void OnProfileChange(ProfileModel profileModel, List<ISpreadSheetSoWrapper> containers);
  }

  public class GoogleSheetsFacade : IDisposable, IGoogleSheetsFacade
  {
    public List<Type> SpreadSheetDataTypes { get; private set; } = new();
    public List<ISpreadSheetSoWrapper> SpreadSheetSoList { get; private set; }
    private readonly IProfilesContainer _profilesContainer;

    private readonly IAutoGenFactory _scriptsFactory;
    private readonly IAutoGenFactory _scriptableObjectFactory;
    private readonly ITypesProvider _typesProvider;

    private IGoogleSheetsDataImporter _googleSheetsDataImporter;
    private IGoogleSheetsReader _googleSheetsReader;
    private readonly ConsoleLogger _consoleLogger;

    public GoogleSheetsFacade(
      IProfilesContainer profilesContainer,
      List<ISpreadSheetSoWrapper> spreadSheetSoList
    )
    {
      _typesProvider = new TypesProvider();
      _consoleLogger = new ConsoleLogger();
      _profilesContainer = profilesContainer;
      if (!_profilesContainer.HasValidProfile) return;

      var currentProfile = _profilesContainer.CurrentProfile;
      if (currentProfile.tableID == string.Empty || currentProfile.credential == null)
      {
        _consoleLogger.LogProfileCredentialNullException(currentProfile.profileName);
        return;
      }

      SetupContainers(spreadSheetSoList);

      _googleSheetsReader = new GoogleSheetsReader(_profilesContainer.CurrentProfile.credential.text);
      _googleSheetsDataImporter = new GoogleSheetsDataImporter(_googleSheetsReader);


      _scriptsFactory = new SpreadSheetCodeFactory(_consoleLogger);
      _scriptableObjectFactory = new SpreadSheetScriptableObjectFactory();
    }


    public void CantFindClassWithAttribute()
    {
      _consoleLogger.LogCantFindClassesWithSpreadSheetAttribute();
    }

    public void SetupContainers(List<ISpreadSheetSoWrapper> list)
    {
      var currentProfile = _profilesContainer.CurrentProfile;
      var typeList =
        _typesProvider.GetClassesWithAttribute<SheetRangeAttribute>(attribute =>
          attribute.Profile == currentProfile.profileName);
      SpreadSheetSoList = list.Where(x => typeList.Contains(x.GetType())).ToList();
    }

    public void OnProfileChange(ProfileModel profileModel, List<ISpreadSheetSoWrapper> containers)
    {
      if (profileModel is not {IsValid: true})
      {
        _consoleLogger.LogProfileCredentialNullException(profileModel?.profileName);
        return;
      }

      _googleSheetsReader = new GoogleSheetsReader(profileModel.credential.text);
      _googleSheetsDataImporter = new GoogleSheetsDataImporter(_googleSheetsReader);
      SetupContainers(containers);
      SearchForSpreadSheets();
    }

    public void SearchForSpreadSheets()
    {
      if (!_profilesContainer.HasValidProfile || !_profilesContainer.CurrentProfile.IsValid) return;
      SpreadSheetDataTypes.Clear();
      SpreadSheetDataTypes = GetProfileDataClass();
    }

    private List<string> GetCurrentProfileTableSheetsNames()
    {
      var tableSheets = _profilesContainer.CurrentProfile.metaData.tableSheetsNames;
      if (tableSheets.Count == 0)
      {
        if (_profilesContainer.CurrentProfile.tableID == string.Empty) return tableSheets;
        _googleSheetsReader.GetTableModel(_profilesContainer.CurrentProfile.tableID).SheetNames
          .ForEach(x => tableSheets.Add(x));
        EditorUtility.SetDirty(_profilesContainer as ScriptableObject);
      }

      return tableSheets;
    }

    public void ClearAllData()
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


    public void CreateAdditionalScripts(Action<bool> isCreatedCallback)
    {
      var currentProfile = _profilesContainer.CurrentProfile;
      _scriptableObjectFactory.Delete(currentProfile.profileName);
      _scriptsFactory.Delete(currentProfile.profileName);
      var isCreated = _scriptsFactory.Create(SpreadSheetDataTypes,
        currentProfile.profileName);
      isCreatedCallback.Invoke(isCreated);
    }

    public void CreateScriptableObjects()
    {
      var currentProfile = _profilesContainer.CurrentProfile;
      _scriptableObjectFactory.Create(_typesProvider.GetClassesWithAttribute<SheetRangeAttribute>(
          attribute => attribute.Profile == currentProfile.profileName),
        currentProfile.profileName);
      //SearchForSpreadSheets();
    }

    public bool CanCreateContainers()
    {
      if (!_profilesContainer.CurrentProfile.metaData.useSoContainers) return false;
      var currentProfile = _profilesContainer.CurrentProfile;
      if (currentProfile.credential == null || currentProfile.tableID == string.Empty) return false;
      var tableSheets = GetCurrentProfileTableSheetsNames();
      var spreadSheetClasses = GetProfileDataClass();

      var sheetRange = _typesProvider.GetClassesWithAttribute<SheetRangeAttribute>(attribute =>
        attribute.Profile == currentProfile.profileName
        && tableSheets.Contains(attribute.DataType.GetAttributeValue((SpreadSheetAttribute st) => st.SheetName)));

      return spreadSheetClasses.Count > sheetRange.Count;
    }

    public void SerializeSheetData()
    {
      if (SpreadSheetDataTypes.Count == 0) return;
      var currentProfile = _profilesContainer.CurrentProfile;
      if (currentProfile.metaData.useSoContainers)
      {
        SerializeSheetDataSoContainers(currentProfile);
      }
      else
      {
        SerializeSheetDataContainers();
      }
    }

    private void SerializeSheetDataSoContainers(ProfileModel currentProfile)
    {
      var containers = SpreadSheetSoList
        .Select(x => x.SheetDataContainer)
        .Where(x => SpreadSheetDataTypes.Contains(x.SheetDataType))
        .ToList();

      if (currentProfile.metaData.loadFromRemote)
      {
        _googleSheetsDataImporter.LoadDataToContainers(
          containers.Where(x => currentProfile.metaData.GetMeta(x.SheetDataType.ToString()).isLoad).ToList(),
          currentProfile.tableID);
        SpreadSheetSoList.ForEach(x => EditorUtility.SetDirty(x as ScriptableObject));
      }

      SerializeContainers(containers);
    }

    private List<Type> GetProfileDataClass()
    {
      var currentProfile = _profilesContainer.CurrentProfile;
      var tableSheets = GetCurrentProfileTableSheetsNames();
      var classesWithAttribute = _typesProvider.GetClassesWithAttribute<SpreadSheetAttribute>(
        attribute => tableSheets.Contains(attribute.SheetName)
                     && (string.IsNullOrEmpty(attribute.Profile) || attribute.Profile == currentProfile.profileName));
      return classesWithAttribute;
    }

    private void SerializeSheetDataContainers()
    {
      var currentProfile = _profilesContainer.CurrentProfile;
      var classesWithAttribute = GetProfileDataClass()
        .Where(x => currentProfile.metaData.GetMeta(x.ToString()).isLoad).ToList();

      var containers = _typesProvider.CreateSheetDataContainers(classesWithAttribute);
      if (containers.Count == 0)
      {
        _consoleLogger.LogCantFindClassesWithSpreadSheetAttribute(_profilesContainer.CurrentProfile.profileName);
        return;
      }

      _googleSheetsDataImporter.LoadDataToContainers(containers, _profilesContainer.CurrentProfile.tableID);
      SerializeContainers(containers);
    }

    public void SerializeContainer(Type dataType)
    {
      var currentProfile = _profilesContainer.CurrentProfile;
      if (currentProfile.metaData.useSoContainers)
      {
        var containers = SpreadSheetSoList
          .Select(x => x.SheetDataContainer)
          .Where(x => dataType == x.SheetDataType)
          .ToList();
        if (currentProfile.metaData.loadFromRemote)
        {
          _googleSheetsDataImporter.LoadDataToContainers(containers, _profilesContainer.CurrentProfile.tableID);
          SpreadSheetSoList.ForEach(x => EditorUtility.SetDirty(x as ScriptableObject));
        }


        SerializeContainers(containers);
      }
      else
      {
        var containers = _typesProvider.CreateSheetDataContainers(new List<Type> {dataType});
        _googleSheetsDataImporter.LoadDataToContainers(containers, _profilesContainer.CurrentProfile.tableID);
        SerializeContainers(containers);
      }
    }

    private void SerializeContainers(List<ISheetDataContainer> containers)
    {
      var spreadSheetsSerializer = new SpreadSheetsSerializer(
        _profilesContainer.CurrentProfile.profileName,
        _profilesContainer.SerializeSettingsContainer);

      spreadSheetsSerializer.SerializationByRule(containers);

      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    public void Dispose()
    {
      _googleSheetsReader?.Dispose();
    }
  }
}