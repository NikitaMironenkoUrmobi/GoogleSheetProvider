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
  public class GoogleSheetsFacade : IDisposable
  {
    public List<Type> SpreadSheetDataTypes { get; } = new();
    public List<ISpreadSheetSoWrapper> SpreadSheetSoList { get; }
    private readonly IProfilesContainer _profilesContainer;

    private readonly IAutoGenFactory _scriptsFactory;
    private readonly IAutoGenFactory _scriptableObjectFactory;
    private readonly ITypesProvider _typesProvider;

    private IGoogleSheetsDataImporter _googleSheetsDataImporter;
    private IGoogleSheetsReader _googleSheetsReader;
    private readonly IConsoleLogger _consoleLogger;

    public GoogleSheetsFacade(
      IProfilesContainer profilesContainer,
      List<ISpreadSheetSoWrapper> spreadSheetSoList
    )
    {
      _profilesContainer = profilesContainer;
      SpreadSheetSoList = spreadSheetSoList;

      _googleSheetsReader = new GoogleSheetsReader(_profilesContainer.CurrentProfile.credential.text);
      _googleSheetsDataImporter = new GoogleSheetsDataImporter(_googleSheetsReader);
      _consoleLogger = new ConsoleLogger();
      _typesProvider = new TypesProvider();
      _scriptsFactory = new SpreadSheetCodeFactory(_consoleLogger);
      _scriptableObjectFactory = new SpreadSheetScriptableObjectFactory();

      _profilesContainer.OnChangeCurrentProfile += OnProfileChange;
    }


    public void CantFindClassWithAttribute()
    {
      _consoleLogger.LogCantFindClassesWithSpreadSheetAttribute();
    }

    private void OnProfileChange(ProfileModel profileModel)
    {
      _googleSheetsReader = new GoogleSheetsReader(profileModel.credential.text);
      _googleSheetsDataImporter = new GoogleSheetsDataImporter(_googleSheetsReader);
      SearchForSpreadSheets();
    }

    public void SearchForSpreadSheets()
    {
      SpreadSheetDataTypes.Clear();
      var tableSheets = GetCurrentProfileTableSheetsNames();
      var types = _typesProvider.GetClassesWithAttribute<SpreadSheetAttribute>(
        attribute => tableSheets.Contains(attribute.SheetName));
      types.ForEach(x => SpreadSheetDataTypes.Add(x));
    }

    private List<string> GetCurrentProfileTableSheetsNames()
    {
      var tableSheets = _profilesContainer.CurrentProfile.metaData.tableSheetsNames;
      if (tableSheets.Count == 0)
      {
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
      var isCreated = _scriptsFactory.Create(_typesProvider.GetClassesWithAttribute<SpreadSheetAttribute>(),
        currentProfile.profileName);
      isCreatedCallback.Invoke(isCreated);
    }

    public void CreateScriptableObjects()
    {
      var currentProfile = _profilesContainer.CurrentProfile;
      _scriptableObjectFactory.Create(_typesProvider.GetClassesWithAttribute<SheetRangeAttribute>(),
        currentProfile.profileName);
      SearchForSpreadSheets();
    }

    public bool CanCreateContainers()
    {
      var tableSheets = GetCurrentProfileTableSheetsNames();
      return _typesProvider
               .GetClassesWithAttribute<SpreadSheetAttribute>(attribute => tableSheets.Contains(attribute.SheetName))
               .Count >
             _typesProvider.GetClassesWithAttribute<SheetRangeAttribute>(attribute =>
                 tableSheets.Contains(attribute.DataType.GetAttributeValue((SpreadSheetAttribute st) => st.SheetName)))
               .Count;
    }

    public void SerializeSheetDataContainers()
    {
      var tableSheets = GetCurrentProfileTableSheetsNames();
      var classesWithAttribute = _typesProvider.GetClassesWithAttribute<SpreadSheetAttribute>(
        attribute => tableSheets.Contains(attribute.SheetName)
      );

      var containers = _typesProvider.CreateSheetDataContainers(classesWithAttribute);
      if (containers.Count == 0)
      {
        _consoleLogger.LogCantFindClassesWithSpreadSheetAttribute(_profilesContainer.CurrentProfile.profileName);
        return;
      }

      _googleSheetsDataImporter.LoadDataToContainers(containers, _profilesContainer.CurrentProfile.tableID);
      SerializeContainers(containers);
    }

    public void SerializeContainer(Type containerDataType)
    {
      var containers = _typesProvider.CreateSheetDataContainers(new List<Type> {containerDataType});
      _googleSheetsDataImporter.LoadDataToContainers(containers, _profilesContainer.CurrentProfile.tableID);
      SerializeContainers(containers);
    }

    public void SerializeFromScriptableObjectContainers()
    {
      SerializeContainers(
        SpreadSheetSoList
          .Select(x => x.SheetDataContainer)
          .ToList());
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
      _profilesContainer.OnChangeCurrentProfile -= OnProfileChange;
      _googleSheetsReader?.Dispose();
    }
  }
}